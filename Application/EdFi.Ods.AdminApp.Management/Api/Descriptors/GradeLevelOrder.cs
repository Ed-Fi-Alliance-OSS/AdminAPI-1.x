// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Api.Descriptors
{
    public static class GradeLevelOrder
    {
        /* Grade levels can have custom sorting defined by the local education agency, however this
        *  sort order data is not available from the API. The below method contains a default sort order
        *  for grade levels with hard coded values copied directly from the ETL source on 2/21/17
        *  Note that the sort order returned may be incorrect if grade levels have changed */
        public static int GetDefaultSortValue(string gradeLevelCodeValue)
        {
            switch (gradeLevelCodeValue)
            {
                case "Early Education":
                    return -3;
                case "Infant/toddler":
                    return -2;
                case "Preschool/Prekindergarten":
                    return -1;
                case "Kindergarten":
                    return 0;
                case "First grade":
                    return 1;
                case "Second grade":
                    return 2;
                case "Third grade":
                    return 3;
                case "Fourth grade":
                    return 4;
                case "Fifth grade":
                    return 5;
                case "Sixth grade":
                    return 6;
                case "Seventh grade":
                    return 7;
                case "Eighth grade":
                    return 8;
                case "Ninth grade":
                    return 9;
                case "Tenth grade":
                    return 10;
                case "Eleventh grade":
                    return 11;
                case "Twelfth grade":
                    return 12;
                case "Postsecondary":
                    return 13;
                case "Ungraded":
                    return 14;
                case "Other":
                    return 15;
                case "Grade 13":
                    return 16;
                case "Adult Education":
                    return 17;
                default:
                    return 18;
            }
        }
    }
}
