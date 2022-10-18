// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace EdFi.Ods.AdminApp.Management.Workflow
{
    public class WorkflowManager<TStep, TContext> 
        where TStep: WorkflowStep<TContext>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(WorkflowManager<,>));

        private readonly TContext _context;
        private readonly CancellationToken _cancellationToken;
        private readonly List<TStep> _steps = new List<TStep>();

        private int _lastStepCompleted = -1;

        public bool Complete { get; private set; }
        public bool Executed { get; private set; }

        public string StatusMessage { get; private set; } = "Operation not yet starated";

        public bool Error { get; private set; }
        public string ErrorMessage { get; private set; } = "";
        public Exception ActionException { get; private set; }

        public bool ErrorDuringRollback { get; private set; }
        public string RollbackErrorMessage { get; private set; } = "";
        public Exception RollbackException { get; private set; }

        public int CurrentStep { get; private set; }
        public int TotalSteps => _steps.Count;
        public int StepsCompletedSuccessfully => _lastStepCompleted + 1;
        public int StepsRolledBack { get; private set; } = 0;

        public string WorkflowName { get; set; }

        private object _syncObject = new object();

        public delegate void JobEventHandler();

        public event JobEventHandler JobStarted;
        public event JobEventHandler JobCompleted;

        public event JobEventHandler StepStarted;
        public event JobEventHandler StepCompleted;

        public WorkflowStatus Status => new WorkflowStatus
        {
            Complete = Complete,
            Executed = Executed,
            CurrentStep = CurrentStep,
            Error = Error,
            ErrorMessage = ErrorMessage,
            StatusMessage = StatusMessage,
            TotalSteps = TotalSteps
        };


        public WorkflowManager(TContext context) : this(context, CancellationToken.None)
        {
        }

        public WorkflowManager(TContext context, CancellationToken cancellationToken)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public WorkflowManager<TStep, TContext> SetWorkflowName(string name)
        {
            WorkflowName = name;
            return this;
        }

        public WorkflowManager<TStep,TContext> AddProcessStep(TStep unitOfWork)
        {
            _steps.Add(unitOfWork);
            return this;
        }

        public WorkflowManager<TStep,TContext> StartWith(TStep unitOfWork)
        {
            if (_steps.Count > 0)
                throw new InvalidOperationException("Additional steps in the process should be added via the ContinueWith method");

            unitOfWork.RollbackPreviousSteps = false; //this is the first step, so no previous steps to roll back
            _steps.Add(unitOfWork);
            return this;
        }

        public WorkflowManager<TStep,TContext> ContinueWith(TStep unitOfWork)
        {
            if (_steps.Count == 0)
                throw new InvalidOperationException("First step in process must be added via the AddProcessStep method");

            _steps.Add(unitOfWork);
            return this;
        }

        public WorkflowResult Execute()
        {
            //A given instance of this class should only be executable one time since we hold
            //on to processing and error state at the end of the run.
            //We're not expecting this class to be used in a multi-threaded situation, but
            //just the same, it's easy to prevent that unintended usage, so we'll do so here.
            lock (_syncObject)
            {
                if (Complete || Executed)
                    throw new InvalidOperationException("The process may only be executed once.");

                Executed = true;
            }

            JobStarted?.Invoke();

            var rollback = false;

            for (var i = 0; i < _steps.Count; ++i)
            {
                CurrentStep = i;
                var step = _steps[i];
                try
                {
                    RunStep(step);
                    _lastStepCompleted = i;
                }
                catch (Exception e)
                {
                    ActionException = e;
                    Error = true;
                    ErrorMessage = step.GetFormattedFailureMessage(e);
                    _logger.Error($"{WorkflowName} - {ErrorMessage}");

                    rollback = true;
                    break;
                }
            }

            if (rollback)
            {
                Rollback();
            }

            Complete = true;

            JobCompleted?.Invoke();

            return new WorkflowResult
            {
                Error = Error,
                ActionException = ActionException,
                ErrorDuringRollback = ErrorDuringRollback,
                ErrorMessage = ErrorMessage,
                RollbackErrorMessage = RollbackErrorMessage,
                RollbackException = RollbackException,
                TotalSteps = TotalSteps,
                StepsCompletedSuccessfully = StepsCompletedSuccessfully,
                StepsRolledBack = StepsRolledBack
            };
        }

        private void RunStep(TStep step)
        {
            StatusMessage = step.StatusMessage;
            _logger.Info($"{WorkflowName} - {StatusMessage}");

            _cancellationToken.ThrowIfCancellationRequested();

            StepStarted?.Invoke();
            step.ExecuteAction?.Invoke(_context);
            StepCompleted?.Invoke();
        }

        private void Rollback()
        {
            RollbackStep(_lastStepCompleted);
        }

        private void RollbackStep(int stepNumber)
        {
            if (stepNumber < 0 || stepNumber >= _steps.Count)
                return;

            var step = _steps[stepNumber];

            try
            {
                CurrentStep = stepNumber;
                RollbackStep(step);
                ++StepsRolledBack;

                if (step.RollbackPreviousSteps && stepNumber > 0)
                {
                    RollbackStep(stepNumber - 1);
                }
            }
            catch (Exception e)
            {
                RollbackException = e;
                ErrorDuringRollback = true;
                RollbackErrorMessage = step.GetFormattedRollbackFailureMessage(e);
                _logger.Error($"{WorkflowName} - {RollbackErrorMessage}");
            }
        }

        private void RollbackStep(TStep step)
        {
            StatusMessage = step.RollbackStatusMessage;
            _logger.Info($"{WorkflowName} - {StatusMessage}");
            StepStarted?.Invoke();
            step.RollBackAction?.Invoke(_context);
            StepCompleted?.Invoke();
        }
    }
}