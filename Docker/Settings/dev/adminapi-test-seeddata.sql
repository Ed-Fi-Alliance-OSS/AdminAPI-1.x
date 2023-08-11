-- LOAD DATAS
INSERT INTO dbo.odsinstances(name, instancetype, connectionstring)
VALUES ('Ods-test', 'OdsInstance', 'Host=localhost; Port=5432; Database=EdFi_Ods; username=username; Integrated Security=true; Application Name=AdminApi;');
