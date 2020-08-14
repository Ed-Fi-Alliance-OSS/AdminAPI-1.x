// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using log4net;

namespace EdFi.Ods.AdminApp.Web
{
    public class Global : System.Web.HttpApplication
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            _logger.Info("AdminApp starting up");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _logger.Info("AdminApp shutting down");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var exception = Server.GetLastError();
                _logger.Error("Global unhandled exception", exception);
            }
            catch (Exception)
            {
            }
        }
    }
}