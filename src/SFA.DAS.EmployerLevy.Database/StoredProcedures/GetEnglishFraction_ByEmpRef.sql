﻿CREATE PROCEDURE [GetEnglishFraction_ByEmpRef]
	@empref varchar(50)
as
select * from
(
	select 
		ef.Id,
		ef.DateCalculated,
		COALESCE(efo.Amount, ef.Amount) AS Amount,
		ef.EmpRef
	from EnglishFraction ef
	outer apply
	(
		select top 1 Amount
		from [EnglishFractionOverride] o
		where o.EmpRef = @empref and o.DateFrom <= DateCalculated
		order by o.DateFrom desc
	) efo
	where EmpRef = @empref
	union
	select 
		o.Id,
		o.DateFrom AS DateCalculated,
		o.Amount,
		o.EmpRef
	from [EnglishFractionOverride] o
	where o.EmpRef = @empref and o.DateFrom <= GETDATE()
) x
order by x.DateCalculated desc
