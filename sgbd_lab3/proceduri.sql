use cofetarie


go
create procedure adaugare1 @descriere varchar(20), @pret int, @nrTelefon varchar(10), @deadline date
as
begin
	begin tran
		begin try
			if(dbo.validate_sortimente(@descriere, @pret) <> 1) or (dbo.validate_comenzi(@nrTelefon, @deadline) <> 1)
			begin
				raiserror('Campuri invalide!', 14, 1)
			end
			declare @sid int
			set @sid = (select count(*) from Sortimente) + 1
			declare @cid int
			set @cid = (select count(*) from Comenzi) + 1
			insert into Sortimente values (@sid, @descriere, @pret)
			insert into Comenzi values (@cid, @nrTelefon, @deadline)
			insert into SortimenteComenzi values (@sid, @cid)
			commit tran
			select 'Tranzactie efectuala cu succes!'
		end try
		begin catch
			rollback tran
			select 'S-a efectuat rollback!'
		end catch
end
go


-- date valide
select * from Sortimente
select * from Comenzi
select * from SortimenteComenzi
exec adaugare1 @descriere = 'Tiramisu', @pret = 10, @nrTelefon = '0745968823', @deadline = '2020-05-05'   -- 'Tranzactie efectuala cu succes!'
select * from Sortimente				-- sid = 1, descriere = 'Tiramisu', pret = 10
select * from Comenzi					-- cid = 1, nrTelefon = '0745968823', deadline = '2020-05-05'
select * from SortimenteComenzi			-- sid = 1, cid = 1


-- date invalide
select * from Sortimente
select * from Comenzi
select * from SortimenteComenzi
exec adaugare1 @descriere = 'Tiramisu', @pret = 10, @nrTelefon = '0245968823', @deadline = '2020-05-05'   -- 'S-a efectuat rollback!'  (nr de telefon este invalid)
select * from Sortimente				-- sid = 1, descriere = 'Tiramisu', pret = 10
select * from Comenzi					-- cid = 1, nrTelefon = '0745968823', deadline = '2020-05-05'
select * from SortimenteComenzi			-- sid = 1, cid = 1


-- date invalide
select * from Sortimente				-- sid = 1, descriere = 'Tiramisu', pret = 10
select * from Comenzi					-- cid = 1, nrTelefon = '0745968823', deadline = '2020-05-05'
select * from SortimenteComenzi			-- sid = 1, cid = 1
exec adaugare1 @descriere = ' ', @pret = 10, @nrTelefon = '0745968823', @deadline = '2020-05-05'   -- 'S-a efectuat rollback!'  (descrierea este invalida)
select * from Sortimente				-- sid = 1, descriere = 'Tiramisu', pret = 10
select * from Comenzi					-- cid = 1, nrTelefon = '0745968823', deadline = '2020-05-05'
select * from SortimenteComenzi			-- sid = 1, cid = 1


--delete from SortimenteComenzi
--delete from Sortimente
--delete from Comenzi


go
create procedure adaugare2 @descriere varchar(20), @pret int, @nrTelefon varchar(10), @deadline date
as
begin
	begin tran
	begin try
		if(dbo.validate_sortimente(@descriere, @pret) <> 1)		-- Sortimente
		begin
			raiserror('Datele pentru sortiment sunt invalide!', 14, 1)
		end
		declare @sid int
		set @sid = (select count(*) from Sortimente) + 1
		insert into Sortimente values (@sid, @descriere, @pret)
		commit tran
		select 'Tranzactie efectuala cu succes pe tabela Sortimente!'
	end try
	begin catch
		rollback tran
		select 'S-a efectuat rollback pe tabela Sortimente!'
	end catch
	begin tran
	begin try
		if(dbo.validate_comenzi(@nrTelefon, @deadline) <> 1)		-- Comenzi
		begin
			raiserror('Datele pentru comanda sunt invalide!', 14, 1)
		end
		declare @cid int
		set @cid = (select count(*) from Comenzi) + 1
		insert into Comenzi values (@cid, @nrTelefon, @deadline)
		commit tran
		select 'Tranzactie efectuala cu succes pe tabela Comenzi!'
	end try
	begin catch
		rollback tran
		select 'S-a efectuat rollback pe tabela Comenzi!'
	end catch
	begin tran
		begin try													-- SortimenteComenzi
			insert into SortimenteComenzi values (@sid, @cid)
			commit tran
		end try
		begin catch
			rollback tran
			select 'S-a efectuat rollback pe tabela SortimenteComenzi!'
		end catch
end
go


-- date valide
select * from Sortimente
select * from Comenzi
select * from SortimenteComenzi
exec adaugare2 @descriere = 'Cheesecake', @pret = 12, @nrTelefon = '0745968343', @deadline = '2020-05-06'   -- 'Tranzactie efectuala cu succes pe tabela Sortimente!' ; 'Tranzactie efectuala cu succes pe tabela Comenzi!'
select * from Sortimente				-- sid = 1, descriere = 'Cheesecake', pret = 12
select * from Comenzi					-- cid = 1, nrTelefon = '0745968343', deadline = '2020-05-06'
select * from SortimenteComenzi			-- sid = 1, cid = 1


delete from SortimenteComenzi
delete from Sortimente
delete from Comenzi


-- date invalide pentru Sortimente
select * from Sortimente
select * from Comenzi
select * from SortimenteComenzi
exec adaugare2 @descriere = '  ', @pret = 15, @nrTelefon = '0723345678', @deadline = '2020-05-05'   -- 'S-a efectuat rollback pe tabela Sortimente!'; 'Tranzactie efectuata cu succes pe tabela Comenzi!'; 'S-a efectuat rollback pe tabela SortimenteComenzi!'
select * from Sortimente				
select * from Comenzi					-- cid = 1, nrTelefon = '0723345678', deadline = '2020-05-05'
select * from SortimenteComenzi			


delete from Comenzi


-- date invalide pentru Comenzi
select * from Sortimente
select * from Comenzi
select * from SortimenteComenzi
exec adaugare2 @descriere = 'Pannacota', @pret = 9, @nrTelefon = '0734555666', @deadline = '2020-10-10'   -- 'Tranzactie efectuata cu succes pe tabela Sortimente!'; 'S-a efectuat rollback pe tabela Comenzi!';  'S-a efectuat rollback pe tabela SortimenteComenzi!'
select * from Sortimente				-- sid = 1, descriere = 'Pannacota', pret = 9
select * from Comenzi					
select * from SortimenteComenzi