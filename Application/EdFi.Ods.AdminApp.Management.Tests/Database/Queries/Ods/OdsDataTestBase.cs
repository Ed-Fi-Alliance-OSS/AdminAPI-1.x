// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Dapper;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using NUnit.Framework;
using Respawn;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Ods.Reports;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Queries.Ods
{
    [TestFixture]
    public abstract class OdsDataTestBase
    {
        protected static TestOdsConnectionProvider TestConnectionProvider = new TestOdsConnectionProvider();
        protected static bool ReportViewsCreated;

        private readonly Checkpoint _checkpoint = new Checkpoint
        {
            CommandTimeout = 300,
            TablesToIgnore = new[]
            {
                "__MigrationHistory"
            },
        };

        private static int GetNewStudentId(string name, int sexDescriptorId)
        {
            name = name ?? "n/a";

            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                var uniqueId = Guid.NewGuid();

                var studentUsi = sqlConnection.Query<int>(@"
                    INSERT INTO edfi.Student(
                         FirstName
                        ,LastSurname
                        ,BirthSexDescriptorId
                        ,BirthDate
                        ,StudentUniqueId
                        ,Id
                        ,LastModifiedDate
                        ,CreateDate)
                    VALUES
                        (@FirstName
                        ,@LastName
                        ,@SexDescriptorId
                        ,@Date
                        ,@UniqueIdString
                        ,@UniqueId
                        ,@Date
                        ,@Date)

                    SELECT  StudentUSI
                    FROM edfi.Student
                    WHERE Id=@UniqueId
                ", new
                {
                    FirstName = name,
                    LastName = name,
                    SexDescriptorId = sexDescriptorId,
                    Date = DateTime.Now,
                    UniqueIdString = uniqueId.ToString().Substring(0,32),
                    UniqueId = uniqueId
                })
                .Single();

                return studentUsi;
            }
        }

        public int GetNewProgramDescriptorId(string name)
        {
            return GetNewDescriptorId(name, "ProgramType");
        }

        public void CreateProgram(int localEducationAgencyId, int programTypeDescriptorId = 1, string programName = "Fake Program")
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Query(@"
                INSERT INTO edfi.Program (EducationOrganizationId, ProgramTypeDescriptorId, ProgramName)
                VALUES (@LocalEducationAgencyId, @ProgramTypeDescriptorId, @ProgramName)",
                new
                {
                    LocalEducationAgencyId = localEducationAgencyId,
                    ProgramTypeDescriptorId = programTypeDescriptorId,
                    ProgramName = programName
                });
            }
        }

        public void EnrollStudentInProgram(int studentUsi, int localEducationAgencyId, int programTypeDescriptorId = 1, string programName = "Fake Program", DateTime? beginDate = null, DateTime? endDate = null)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Query(@"
                INSERT INTO edfi.GeneralStudentProgramAssociation (StudentUSI, EducationOrganizationId, ProgramTypeDescriptorId, ProgramName, ProgramEducationOrganizationId, BeginDate, EndDate)
                VALUES (@StudentUSI, @LocalEducationAgencyId, @ProgramTypeDescriptorId, @ProgramName, @LocalEducationAgencyId, @BeginDate, @EndDate)", 
                new
                {
                    StudentUSI = studentUsi,
                    LocalEducationAgencyId = localEducationAgencyId,
                    ProgramTypeDescriptorId = programTypeDescriptorId,
                    ProgramName = programName,
                    BeginDate = beginDate ?? DateTime.Now,
                    EndDate = endDate
                });
            }
        }

        public TestStudentWithSchoolAssociation GetNewStudentWithSchoolAssociation(
            int? schoolId = null,
            string name = null,
            int? sexDescriptorId = null,
            bool? isHispanicLatino = false)
        {
            int localEducationAgencyId;
            if (schoolId == null)
            {
                localEducationAgencyId = GetNewLocalEducationAgencyId();
                schoolId = GetNewSchool(localEducationAgencyId);
            }
            else
            {
                localEducationAgencyId = GetLocalEducationAgencyIdFromSchool(schoolId.Value);
            }

            sexDescriptorId = sexDescriptorId ?? GetNewSexDescriptorId(Guid.NewGuid().ToString());
            var studentUsi = GetNewStudentId(name, sexDescriptorId.Value);
            AssociateStudentWithSchool(studentUsi, schoolId.Value);
            AssociateStudentWithEdOrg(studentUsi, localEducationAgencyId, sexDescriptorId.Value, isHispanicLatino);

            return new TestStudentWithSchoolAssociation
            {
                StudentUsi = studentUsi,
                LocalEducationAgencyId = localEducationAgencyId,
                SchoolId = schoolId.Value,
            };
        }

        public int GetNewSexDescriptorId(string name)
        {
            return GetNewDescriptorId(name, "Sex");
        }

        public int GetNewRaceDescriptorId(string name)
        {
            return GetNewDescriptorId(name, "Race");
        }

        private static void AssociateStudentWithEdOrg(int studentUsi, int edOrgId, int sexDescriptorId, bool? isHispanicLatino)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                    INSERT INTO edfi.StudentEducationOrganizationAssociation
                       (StudentUSI
                       ,EducationOrganizationId
                       ,SexDescriptorId
                       ,HispanicLatinoEthnicity
                       ,CreateDate)
                    VALUES
                       (@StudentUSI
                       ,@EducationOrganizationId
                       ,@SexDescriptorId
                       ,@HispanicLatinoEthnicity
                       ,@Date)
                    ", 
                    new
                    {
                        StudentUSI = studentUsi,
                        EducationOrganizationId = edOrgId,
                        SexDescriptorId = sexDescriptorId,
                        HispanicLatinoEthnicity = isHispanicLatino,
                        Date = DateTime.Now
                    });
            }
        }

        public void SetStudentRace(int studentUsi, int edOrgId, int raceDescriptorId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                    INSERT INTO edfi.StudentEducationOrganizationAssociationRace
                       (StudentUSI
                       ,EducationOrganizationId
                       ,RaceDescriptorId
                       ,CreateDate)
                    VALUES
                       (@StudentUSI
                       ,@EducationOrganizationId
                       ,@RaceDescriptorId
                       ,@Date)
                    ", 
                    new
                    {
                        StudentUSI = studentUsi,
                        EducationOrganizationId = edOrgId,
                        RaceDescriptorId = raceDescriptorId,
                        Date = DateTime.Now
                    });
            }
        }

        public int GetNewSchool(int? localEducationAgencyId = null, string name = null)
        {
            name = name ?? "n/a";
            localEducationAgencyId = localEducationAgencyId ?? GetNewLocalEducationAgencyId();
            var edOrgId = GetNewEducationOrganizationId(name);
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                INSERT INTO edfi.School
                           (SchoolId
                           ,LocalEducationAgencyId
                           ,SchoolTypeDescriptorId
                           ,CharterStatusDescriptorId
                           ,TitleIPartASchoolDesignationDescriptorId
                           ,MagnetSpecialProgramEmphasisSchoolDescriptorId
                           ,AdministrativeFundingControlDescriptorId
                           ,InternetAccessDescriptorId
                           ,CharterApprovalAgencyTypeDescriptorId
                           ,CharterApprovalSchoolYear)
                     VALUES
                           (@EdOrgId
                           ,@LocalEducationAgencyId
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null)
                    ", new { EdOrgId = edOrgId, Name = name, LocalEducationAgencyId = localEducationAgencyId });
                return edOrgId;
            }
        }

        public int GetNewSchoolCategoryDescriptorId(string name)
        {
            return GetNewDescriptorId(name, "SchoolCategory");
        }

        public void SetSchoolCategoryDescriptor(int schoolId, int schoolCategoryDescriptorId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                INSERT INTO edfi.SchoolCategory (SchoolId, SchoolCategoryDescriptorId)
                VALUES (@SchoolId, @SchoolCategoryDescriptorId)
                ", new {SchoolId = schoolId, SchoolCategoryDescriptorId = schoolCategoryDescriptorId});
            }
        }

        private static int GetLocalEducationAgencyIdFromSchool(int schoolId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                return
                    sqlConnection.Query<int>
                        ("SELECT LocalEducationAgencyID FROM edfi.school WHERE SchoolID = @SchoolID"
                            , new {SchoolId = schoolId})
                        .FirstOrDefault();
            }
        }

        public int GetNewLocalEducationAgencyId(string name = null)
        {
            name = name ?? "n/a";
            var edOrgId = GetNewEducationOrganizationId(name);
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                var localEducationAgencyCategoryDescriptorId = GetNewDescriptorId(descriptorName: "LocalEducationAgencyCategory");

                sqlConnection.Execute(@"
                INSERT INTO edfi.LocalEducationAgency
                           (LocalEducationAgencyId
                           ,LocalEducationAgencyCategoryDescriptorId
                           ,CharterStatusDescriptorId
                           ,ParentLocalEducationAgencyId
                           ,EducationServiceCenterId
                           ,StateEducationAgencyId)
                     VALUES
                           (@LocalEducationAgencyId
                           ,@LocalEducationAgencyCategoryDescriptorId
                           ,null
                           ,null
                           ,null
                           ,null)
                ", new { LocalEducationAgencyId = edOrgId, LocalEducationAgencyCategoryDescriptorId = localEducationAgencyCategoryDescriptorId});
                return edOrgId;
            }
        }

        private static int GetNewEducationOrganizationId(string name)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                var edOrgId = sqlConnection.Query<int>("SELECT ISNULL(MAX(EducationOrganizationId),0) + 1 FROM edfi.EducationOrganization").Single();
                sqlConnection.Execute(@"
                    INSERT INTO edfi.EducationOrganization(
                               EducationOrganizationId
                               ,NameOfInstitution
                               ,ShortNameOfInstitution
                               ,WebSite
                               ,OperationalStatusDescriptorId
                               ,Id
                               ,LastModifiedDate
                               ,CreateDate)
                         VALUES(
                                @EdOrgId
                               ,@Name
                               ,@Name
                               ,'n/a'
                               ,null
                               ,@UniqueIdentifier
                               ,@Date
                               ,@Date)
                    ", new { EdOrgId = edOrgId, Name = name, Date = DateTime.Now, UniqueIdentifier = Guid.NewGuid()});
                return edOrgId;
            }
        }

        private void AssociateStudentWithSchool(int studentUsi, int schoolId)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                INSERT INTO edfi.StudentSchoolAssociation
                           (StudentUSI
                           ,SchoolId
                           ,SchoolYear
                           ,EntryDate
                           ,EntryGradeLevelDescriptorId
                           ,EntryGradeLevelReasonDescriptorId
                           ,EntryTypeDescriptorId
                           ,RepeatGradeIndicator
                           ,SchoolChoiceTransfer
                           ,ExitWithdrawDate
                           ,ExitWithdrawTypeDescriptorId
                           ,ResidencyStatusDescriptorId
                           ,PrimarySchool
                           ,EmployedWhileEnrolled
                           ,ClassOfSchoolYear
                           ,EducationOrganizationId
                           ,GraduationPlanTypeDescriptorId
                           ,GraduationSchoolYear
                           ,Id
                           ,LastModifiedDate
                           ,CreateDate)
                     VALUES
                           (@StudentUsi
                           ,@SchoolId
                           ,@SchoolYear
                           ,@Date
                           ,@EntryGradeLevelDescriptorId
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,null
                           ,@Id
                           ,@Date
                           ,@Date)
                ", new {StudentUsi = studentUsi,
                    SchoolId = schoolId,
                    SchoolYear = GetNewSchoolYear(),
                    Date = DateTime.Now,
                    EntryGradeLevelDescriptorId = GetNewGradeLevelDescriptorId(),
                    Id = Guid.NewGuid()
                });
            }
        }

        protected static short GetNewSchoolYear(short? year = null)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                var schoolYear = year ?? sqlConnection.Query<short>("SELECT ISNULL(MAX(SchoolYear),1990) + 1 FROM edfi.SchoolYearType").Single();
                sqlConnection.Execute(@"
                    INSERT INTO edfi.SchoolYearType
                               (SchoolYear
                               ,SchoolYearDescription
                               ,CurrentSchoolYear
                               ,Id
                               ,LastModifiedDate
                               ,CreateDate)
                         VALUES
                               (@SchoolYearId
                               ,@SchoolYearDescription
                               ,@IsCurrent
                               ,@Id
                               ,@Date
                               ,@Date)
                    ", 
                    new {
                        SchoolYearId = schoolYear,
                        SchoolYearDescription = $"{schoolYear-1}-{schoolYear}",
                        IsCurrent = false,
                        Id = Guid.NewGuid(),
                        Date = DateTime.Now
                    });

                return schoolYear;
            }
        }

        private int GetNewGradeLevelDescriptorId()
        {
            var gradeLevelDescriptorId = GetNewDescriptorId(descriptorName: "GradeLevel");
            return gradeLevelDescriptorId;
        }

        public int GetNewDescriptorId(string description = null, string descriptorName = null)
        {
            description = description ?? Guid.NewGuid().ToString();
            int descriptorId;
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                descriptorId = sqlConnection.Query<int>(@"
                INSERT INTO edfi.Descriptor
                           (Namespace
                           ,CodeValue
                           ,ShortDescription
                           ,Description
                           ,PriorDescriptorId
                           ,EffectiveBeginDate
                           ,EffectiveEndDate
                           ,Id
                           ,LastModifiedDate
                           ,CreateDate)
                     VALUES
                           ('n/a'
                           ,@Description
                           ,@Description
                           ,@Description
                           ,null
                           ,null
                           ,null
                           ,@Id
                           ,@Date
                           ,@Date)
                    SELECT DescriptorId
                    FROM edfi.Descriptor
                    WHERE Id = @Id
                ", new { Description = description, Id = Guid.NewGuid(), Date = DateTime.Now})
                .Single();
            }

            if (!string.IsNullOrEmpty(descriptorName))
            {
                JoinDescriptorWithDescriptorTable(descriptorId, descriptorName);
            }

            return descriptorId;
        }

        public static void SetStudentInactive(int studentUsi, int schoolId, DateTime? inactiveDate = null)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                sqlConnection.Execute(@"
                UPDATE edfi.StudentSchoolAssociation
                SET ExitWithdrawDate = @Date
                WHERE StudentUSI = @StudentUSI
                    AND SchoolId = @SchoolId
                ", new { SchoolId = schoolId, StudentUSI = studentUsi, Date = inactiveDate ?? DateTime.Now.AddDays(-1) });
            }
        }

        private static void JoinDescriptorWithDescriptorTable(int descriptorId, string descriptorName)
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                var command = string.Format(@"
                    INSERT INTO edfi.{0}Descriptor
                       ({0}DescriptorId)
                    VALUES
                       (@DescriptorId)
                ", descriptorName);

                sqlConnection.Execute(command, new { DescriptorId = descriptorId });
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            await _checkpoint.Reset(TestOdsConnectionProvider.ConnectionString);
        }

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            var mockReportConfigProvider = new Mock<IReportsConfigProvider>();
            mockReportConfigProvider.Setup(x => x.Create(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox)).Returns(new ReportsConfig
                { ConnectionString = TestOdsConnectionProvider.ConnectionString, ScriptFolder = "Reports.Sql" });

            Scoped<IUpgradeEngineFactory>(upgradeEngineFactory =>
            {
                var reportViews = new ReportViewsSetUp(mockReportConfigProvider.Object, upgradeEngineFactory);
                reportViews.CreateReportViews(CloudOdsDatabaseNames.ProductionOds, ApiMode.Sandbox);
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DropCreatedReportViews();
        }

        private static void DropCreatedReportViews()
        {
            using (var sqlConnection = TestConnectionProvider.CreateNewConnection(null))
            {
                const string dropViewsCommand = @"select 'DROP VIEW  IF EXISTS' + QUOTENAME(sc.name) + '.' + QUOTENAME(obj.name) + ';'
                                        FROM sys.objects obj
                                        INNER JOIN sys.schemas sc
                                        ON sc.schema_id = obj.schema_id
                                        WHERE obj.type='V'
                                        AND sc.name = 'adminapp';";

                var dropViewList = sqlConnection.QueryAsync<string>(dropViewsCommand).Result;
                foreach (var dropView in dropViewList)
                {
                    sqlConnection.Execute(dropView);
                }
                const string dropSchemaCommand = "DROP SCHEMA IF EXISTS adminapp;";
                sqlConnection.Execute(dropSchemaCommand);
            }
        }
    }

    public class TestOdsConnectionProvider : IDatabaseConnectionProvider
    {
        public static string ConnectionString =
            Scoped<IConfiguration, string>(configuration => configuration["ConnectionStrings:OdsEmpty"]);

        public IDbConnection CreateNewConnection(int odsInstanceNumericSuffix, ApiMode apiMode)
        {
            throw new NotImplementedException();
        }

        public IDbConnection CreateNewConnection(string odsInstanceName = "EdFi_Ods_Production", ApiMode apiMode = null)
        {
            return new SqlConnection(ConnectionString);
        }
    }

    public class TestStudentWithSchoolAssociation
    {
        public int StudentUsi { get; set; }
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
    }
}
