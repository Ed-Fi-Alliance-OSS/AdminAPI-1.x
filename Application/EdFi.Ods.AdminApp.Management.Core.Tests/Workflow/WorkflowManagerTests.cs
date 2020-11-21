// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading;
using EdFi.Ods.AdminApp.Management.Workflow;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Workflow
{
    [TestFixture]
    public class WorkflowManagerTests
    {
        private class TestWorkflowStep : WorkflowStep<int>
        {   
        }

        [Test]
        public void ShouldThrowIfExecutedTwice()
        {
            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(GetSuccessfulUnitOfWork())
                .ContinueWith(GetSuccessfulUnitOfWork())
                .ContinueWith(GetSuccessfulUnitOfWork());

            var result = workflowManager.Execute();
            Assert.Throws<InvalidOperationException>(() => workflowManager.Execute());
        }


        [Test]
        public void ShouldCallAllSteps()
        {
            var action1Called = false;
            var action2Called = false;
            var action3Called = false;

            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(new TestWorkflowStep {ExecuteAction = conn => { action1Called = true; }})
                .ContinueWith(new TestWorkflowStep {ExecuteAction = conn => { action2Called = true; }})
                .ContinueWith(new TestWorkflowStep {ExecuteAction = conn => { action3Called = true; }});

            var result = workflowManager.Execute();

            result.TotalSteps.ShouldBe(3);
            result.StepsCompletedSuccessfully.ShouldBe(3);
            result.StepsRolledBack.ShouldBe(0);
            result.Error.ShouldBeFalse();
            result.ActionException.ShouldBeNull();
            result.ErrorMessage.ShouldBeEmpty();
            result.ErrorDuringRollback.ShouldBeFalse();
            result.RollbackException.ShouldBeNull();
            result.RollbackErrorMessage.ShouldBeEmpty();

            action1Called.ShouldBeTrue();
            action2Called.ShouldBeTrue();
            action3Called.ShouldBeTrue();
        }
        

        [Test]
        public void ShouldAttemptRollbackIfActionFails()
        {
            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(GetSuccessfulUnitOfWork())
                .ContinueWith(GetSuccessfulUnitOfWork())
                .ContinueWith(GetUnitOfWorkThatErrors());

            var result = workflowManager.Execute();

            result.TotalSteps.ShouldBe(3);
            result.StepsCompletedSuccessfully.ShouldBe(2);
            result.StepsRolledBack.ShouldBe(2);
            result.Error.ShouldBeTrue();
            result.ActionException.ShouldNotBeNull();
            result.ErrorMessage.ShouldBe("Process failed");
            result.ErrorDuringRollback.ShouldBeFalse();
            result.RollbackException.ShouldBeNull();
            result.RollbackErrorMessage.ShouldBeEmpty();
        }

        [Test]
        public void RollbackShouldNotRunForStepThatJustFailed()
        {
            var rollbackCalled = false;
            var uow = new TestWorkflowStep
            {
                ExecuteAction = conn => { throw new Exception(); },
                RollBackAction = conn => { rollbackCalled = true; },
                FailureMessage = "Process failed",
                RollbackFailureMessage = "Should never fail",
                RollbackPreviousSteps = true
            };

            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(GetSuccessfulUnitOfWork())
                .ContinueWith(uow);

            var result = workflowManager.Execute();

            result.TotalSteps.ShouldBe(2);
            result.StepsCompletedSuccessfully.ShouldBe(1);
            result.StepsRolledBack.ShouldBe(1);
            result.Error.ShouldBeTrue();
            result.ActionException.ShouldNotBeNull();
            result.ErrorMessage.ShouldBe("Process failed");
            result.ErrorDuringRollback.ShouldBeFalse();
            result.RollbackException.ShouldBeNull();
            result.RollbackErrorMessage.ShouldBeEmpty();
            rollbackCalled.ShouldBeFalse();
        }

        [Test]
        public void ShouldStopIfRollbackActionFails()
        {
            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(GetSuccessfulUnitOfWork())
                .ContinueWith(GetUnitOfWorkThatErrorsDuringRollback())
                .ContinueWith(GetUnitOfWorkThatErrors());

            var result = workflowManager.Execute();

            result.TotalSteps.ShouldBe(3);
            result.StepsCompletedSuccessfully.ShouldBe(2);
            result.StepsRolledBack.ShouldBe(0);
            result.Error.ShouldBeTrue();
            result.ActionException.ShouldNotBeNull();
            result.ErrorMessage.ShouldBe("Process failed");
            result.ErrorDuringRollback.ShouldBeTrue();
            result.RollbackException.ShouldNotBeNull();
            result.RollbackErrorMessage.ShouldBe("Rollback failed");
        }

        [Test]
        public void ShouldNotRollbackPreviousStepsIfIndicated()
        {
            var uowThatStopsRollback = GetSuccessfulUnitOfWork();
            uowThatStopsRollback.RollbackPreviousSteps = false;

            var rollBackErroneouslyCalled = false;
            var uowThatShouldNotBeRolledBack = GetSuccessfulUnitOfWork();
            uowThatShouldNotBeRolledBack.RollBackAction = conn => { rollBackErroneouslyCalled = true; };

            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0);
            workflowManager.StartWith(uowThatShouldNotBeRolledBack)
                .ContinueWith(GetUnitOfWorkThatErrorsDuringRollback())
                .ContinueWith(uowThatStopsRollback)
                .ContinueWith(GetUnitOfWorkThatErrors());

            var result = workflowManager.Execute();

            result.TotalSteps.ShouldBe(4);
            result.StepsCompletedSuccessfully.ShouldBe(3);
            result.StepsRolledBack.ShouldBe(1);
            result.Error.ShouldBeTrue();
            result.ActionException.ShouldNotBeNull();
            result.ErrorMessage.ShouldBe("Process failed");
            result.ErrorDuringRollback.ShouldBeFalse();
            result.RollbackException.ShouldBeNull();
            result.RollbackErrorMessage.ShouldBeEmpty();

            rollBackErroneouslyCalled.ShouldBeFalse();
        }

        [Test]
        public void ShouldCancelIfRequested()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var uowThatCancels = GetSuccessfulUnitOfWork();
            uowThatCancels.ExecuteAction = (conn) => { cancellationTokenSource.Cancel(); };
            
            var workflowManager = new WorkflowManager<TestWorkflowStep, int>(0, cancellationTokenSource.Token);
            workflowManager.StartWith(GetSuccessfulUnitOfWork())
                .ContinueWith(uowThatCancels)
                .ContinueWith(GetSuccessfulUnitOfWork());

            var result = workflowManager.Execute();
            result.Error.ShouldBe(true);
            result.StepsCompletedSuccessfully.ShouldBe(2); //uowThatCancels should succeed since the check for cancellation happens before the step action is called
            result.StepsRolledBack.ShouldBe(2);
        }

        private TestWorkflowStep GetSuccessfulUnitOfWork()
        {
            return new TestWorkflowStep
            {
                ExecuteAction = conn => { },
                RollBackAction = conn => { },
                FailureMessage = "Should never fail",
                RollbackFailureMessage = "Should never fail",
                RollbackPreviousSteps = true
            };
        }

        private TestWorkflowStep GetUnitOfWorkThatErrors()
        {
            return new TestWorkflowStep
            {
                ExecuteAction = conn => { throw new Exception(); },
                RollBackAction = conn => { },
                FailureMessage = "Process failed",
                RollbackFailureMessage = "Should never fail",
                RollbackPreviousSteps = true
            };
        }

        private TestWorkflowStep GetUnitOfWorkThatErrorsDuringRollback()
        {
            return new TestWorkflowStep
            {
                ExecuteAction = conn => { },
                RollBackAction = conn => { throw new Exception(); },
                FailureMessage = "Should never fail",
                RollbackFailureMessage = "Rollback failed",
                RollbackPreviousSteps = true
            };
        }
    }
}
