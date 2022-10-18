// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Settings
{
    public sealed class LogLevel : Enumeration<LogLevel>
    {
        public static readonly LogLevel All = new LogLevel(0, "ALL");
        public static readonly LogLevel Debug = new LogLevel(1, "DEBUG");
        public static readonly LogLevel Info = new LogLevel(2, "INFO");
        public static readonly LogLevel Warn = new LogLevel(3, "WARN");
        public static readonly LogLevel Error = new LogLevel(4, "ERROR");
        public static readonly LogLevel Fatal = new LogLevel(5, "FATAL");
        public static readonly LogLevel Off = new LogLevel(6, "OFF");

        private LogLevel(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
