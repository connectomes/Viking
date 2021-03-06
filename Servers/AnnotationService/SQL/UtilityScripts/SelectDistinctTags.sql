/****** Script for SelectTopNRows command from SSMS  ******/
select distinct N.value('.','nvarchar(128)') as TagName
from Structure
    cross apply Tags.nodes('Structure/Attrib/@Name') as T(N)
ORDER BY TagName