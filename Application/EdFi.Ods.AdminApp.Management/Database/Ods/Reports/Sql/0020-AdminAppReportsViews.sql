-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

--
-- Name: StudentDemographicsReport; Type: VIEW; Schema: adminapp; 
--

CREATE OR ALTER VIEW adminapp.StudentDemographicsReport 
AS
SELECT DISTINCT s.studentUniqueId,
    sd.ShortDescription AS Gender,
    seoa.HispanicLatinoEthnicity,
    rd.ShortDescription AS StudentRace,
    seoa.EducationOrganizationId,
    sch.LocalEducationAgencyId
FROM edfi.Student AS s 
INNER JOIN edfi.StudentSchoolAssociation AS ssa ON 
    s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School sch ON 
    ssa.SchoolId = sch.SchoolId
INNER JOIN edfi.StudentEducationOrganizationAssociation seoa ON 
    seoa.StudentUSI = s.StudentUSI
LEFT JOIN edfi.Descriptor sd ON
    seoa.SexDescriptorId = sd.DescriptorId
LEFT JOIN edfi.StudentEducationOrganizationAssociationRace seoar ON
    seoar.StudentUSI = s.StudentUSI 
    AND seoar.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN edfi.Descriptor rd ON 
    seoar.RaceDescriptorId = rd.DescriptorId
WHERE ((ssa.ExitWithdrawDate IS NULL) OR (ssa.ExitWithdrawDate >= GETDATE()));

GO

--
-- Name: LocalEducationAgenciesReport; Type: VIEW; Schema: adminapp; 
--

CREATE OR ALTER VIEW adminapp.LocalEducationAgenciesReport 
AS
SELECT LocalEducationAgencyId, 
       NameOfInstitution AS Name
FROM edfi.EducationOrganization e
inner join edfi.LocalEducationAgency l
ON l.LocalEducationAgencyId = e.EducationOrganizationId

GO

--
-- Name: DistrictTotalEnrollmentsReport; Type: VIEW; Schema: adminapp;
--

CREATE OR ALTER VIEW adminapp.DistrictTotalEnrollmentsReport 
AS
SELECT COUNT(DISTINCT s.StudentUSI) AS TotalEnrollment, 
            sch.LocalEducationAgencyId
FROM edfi.Student AS s
INNER JOIN edfi.StudentSchoolAssociation AS ssa ON s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School AS sch ON (ssa.SchoolId = sch.SchoolId) 
WHERE (ssa.ExitWithdrawDate IS NULL) OR (ssa.ExitWithdrawDate >= GETDATE())
GROUP BY sch.LocalEducationAgencyId;

GO

--
-- Name: DistrictSchoolsByTypeReport; Type: VIEW; Schema: adminapp; 
--

CREATE OR ALTER VIEW adminapp.DistrictSchoolsByTypeReport
AS
SELECT d.ShortDescription AS Description, 
      COUNT(*) AS Count, 
	  LocalEducationAgencyId
FROM edfi.School s JOIN edfi.schoolcategory sc
ON s.SchoolId = sc.SchoolId JOIN edfi.Descriptor d
ON d.DescriptorId = sc.SchoolCategoryDescriptorId
GROUP BY d.ShortDescription, LocalEducationAgencyId;

GO

--
-- Name: StudentsByProgramReport; Type: VIEW; Schema: adminapp; 
--

CREATE OR ALTER VIEW adminapp.StudentsByProgramReport
AS
SELECT spa.ProgramName, 
       spa.EducationOrganizationId, 
	   sch.LocalEducationAgencyId
FROM edfi.Student AS s
INNER JOIN edfi.StudentSchoolAssociation AS ssa ON s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School AS sch ON (ssa.SchoolId = sch.SchoolId) 
INNER JOIN edfi.GeneralStudentProgramAssociation AS spa ON (spa.StudentUSI = s.StudentUSI)
WHERE ((spa.EndDate > GETDATE()) OR (spa.EndDate IS NULL))
AND ((ssa.ExitWithdrawDate IS NULL) OR (ssa.ExitWithdrawDate >= GETDATE()));

GO

--
-- Name: StudentEconomicSituationReport; Type: VIEW; Schema: adminapp;
--

CREATE OR ALTER VIEW adminapp.StudentEconomicSituationReport
AS
SELECT DISTINCT s.StudentUniqueId,
(CASE WHEN atRisk.StudentUSI IS NOT NULL THEN 1 ELSE 0 END) as AtRisk,
(CASE WHEN economicDisadvantaged.StudentUSI IS NOT NULL THEN 1 ELSE 0 END) as EconomicallyDisadvantaged,
(CASE WHEN sfd.ShortDescription = 'Free' OR sfd.ShortDescription = 'Reduced Price' THEN 1 ELSE 0 END) AS FreeOrReducedPriceLunchEligible,
(CASE WHEN migrant.StudentUSI IS NOT NULL THEN 1 ELSE 0 END) as Migrant,
(CASE WHEN immigrant.StudentUSI IS NOT NULL THEN 1 ELSE 0 END) as Immigrant,
(CASE WHEN homeless.StudentUSI IS NOT NULL THEN 1 ELSE 0 END) as Homeless,
(CASE WHEN lepd.ShortDescription = 'Limited' THEN 1 ELSE 0 END) as LimitedEnglishProficency,
seoa.EducationOrganizationId,
sch.LocalEducationAgencyId
FROM edfi.Student AS s
INNER JOIN edfi.StudentSchoolAssociation AS ssa ON s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School AS sch ON ssa.SchoolId = sch.SchoolId
INNER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
LEFT JOIN (
    SELECT DISTINCT si.StudentUSI, si.EducationOrganizationId, si.IndicatorName
    FROM edfi.StudentEducationOrganizationAssociationStudentIndicator si
    WHERE si.IndicatorName = 'At Risk'
) atRisk ON atRisk.StudentUSI = s.StudentUSI AND atRisk.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN edfi.StudentSchoolFoodServiceProgramAssociationSchoolFoodServiceProgramService fsps ON fsps.StudentUSI = s.StudentUSI AND fsps.EducationOrganizationId = sch.SchoolId
LEFT JOIN edfi.SchoolFoodServiceProgramServiceDescriptor AS sfsed ON fsps.SchoolFoodServiceProgramServiceDescriptorId = sfsed.SchoolFoodServiceProgramServiceDescriptorId
LEFT JOIN edfi.Descriptor sfd ON sfsed.SchoolFoodServiceProgramServiceDescriptorId = sfd.DescriptorId
LEFT JOIN (
    SELECT DISTINCT seoasc.StudentUSI, seoasc.EducationOrganizationId
    FROM edfi.StudentEducationOrganizationAssociationStudentCharacteristic seoasc INNER JOIN edfi.StudentCharacteristicDescriptor AS scd ON 
    seoasc.StudentCharacteristicDescriptorId = scd.StudentCharacteristicDescriptorId INNER JOIN edfi.Descriptor ON scd.StudentCharacteristicDescriptorId = Descriptor.DescriptorId
    WHERE Descriptor.ShortDescription = 'Economic Disadvantaged'
) economicDisadvantaged ON economicDisadvantaged.StudentUSI = s.StudentUSI AND economicDisadvantaged.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN (
    SELECT DISTINCT seoasc.StudentUSI, seoasc.EducationOrganizationId
    FROM edfi.StudentEducationOrganizationAssociationStudentCharacteristic seoasc INNER JOIN edfi.StudentCharacteristicDescriptor AS scd ON 
    seoasc.StudentCharacteristicDescriptorId = scd.StudentCharacteristicDescriptorId INNER JOIN edfi.Descriptor ON scd.StudentCharacteristicDescriptorId = Descriptor.DescriptorId
    WHERE Descriptor.ShortDescription = 'Migrant'
) migrant ON migrant.StudentUSI = s.StudentUSI AND migrant.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN (
    SELECT DISTINCT seoasc.StudentUSI, seoasc.EducationOrganizationId
    FROM edfi.StudentEducationOrganizationAssociationStudentCharacteristic seoasc INNER JOIN edfi.StudentCharacteristicDescriptor AS scd ON 
    seoasc.StudentCharacteristicDescriptorId = scd.StudentCharacteristicDescriptorId INNER JOIN edfi.Descriptor ON scd.StudentCharacteristicDescriptorId = Descriptor.DescriptorId
    WHERE Descriptor.ShortDescription = 'Immigrant'
) immigrant ON immigrant.StudentUSI = s.StudentUSI AND immigrant.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN (
    SELECT DISTINCT seoasc.StudentUSI, seoasc.EducationOrganizationId
    FROM edfi.StudentEducationOrganizationAssociationStudentCharacteristic seoasc INNER JOIN edfi.StudentCharacteristicDescriptor AS scd ON 
    seoasc.StudentCharacteristicDescriptorId = scd.StudentCharacteristicDescriptorId INNER JOIN edfi.Descriptor ON scd.StudentCharacteristicDescriptorId = Descriptor.DescriptorId
    WHERE Descriptor.ShortDescription = 'Homeless'
) homeless ON homeless.StudentUSI = s.StudentUSI AND homeless.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN edfi.studenteducationorganizationassociationlanguageuse seoal ON seoal.StudentUSI = s.StudentUSI AND seoal.EducationOrganizationId = seoa.EducationOrganizationId
LEFT JOIN edfi.LanguageDescriptor ld ON seoal.LanguageDescriptorId = ld.LanguageDescriptorId
LEFT JOIN edfi.Descriptor ldd ON ldd.DescriptorId = ld.LanguageDescriptorId
LEFT JOIN edfi.LanguageUseDescriptor lud ON seoal.LanguageUseDescriptorId = lud.LanguageUseDescriptorId
LEFT JOIN edfi.Descriptor lds ON lud.LanguageUseDescriptorId = lds.DescriptorId
LEFT JOIN edfi.StudentEducationOrganizationAssociationRace seoar ON seoar.EducationOrganizationId = sch.SchoolId AND seoar.StudentUSI = s.StudentUSI
LEFT JOIN edfi.RaceDescriptor rd on seoar.RaceDescriptorId = rd.RaceDescriptorId
LEFT JOIN edfi.Descriptor rdd ON rdd.DescriptorId = rd.RaceDescriptorId
LEFT JOIN edfi.Descriptor lepd ON seoa.LimitedEnglishProficiencyDescriptorId = lepd.DescriptorId
WHERE ((ssa.ExitWithdrawDate IS NULL) OR (ssa.ExitWithdrawDate >= GETDATE()))

GO