use cofetarie
go

create function dbo.validate_sortimente(@descriere varchar(20), @pret int) returns int
as
begin
	declare @deReturnat int
	set @deReturnat = 1
	if trim(@descriere) = ''
		set @deReturnat = 0
	if @pret <= 0 or @pret >= 100
		set @deReturnat = 0
	return @deReturnat
end
go


create function dbo.validate_comenzi(@nrTelefon varchar(10), @deadline date) returns int
as
begin
	declare @deReturnat int
	set @deReturnat = 1
	if SUBSTRING(@nrTelefon, 1, 2) <> '07'
		set @deReturnat = 0
	if datediff(day, GETDATE(), @deadline) <= 0   --     deadline sa fie cu cel putin o zi mai mare decat data curenta
		set @deReturnat = 0
	if datediff(day, GETDATE(), @deadline) > 50   --     deadline - data_curenta sa fie <= 50 (se pot face comenzi cu maxim 50 de zile in avans)
		set @deReturnat = 0
	return @deReturnat
end
go
