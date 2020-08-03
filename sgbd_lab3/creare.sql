create database cofetarie

use cofetarie

create table Sortimente(
	sid int primary key,
	descriere varchar(20),
	pret int      -- se va considera pentru 100 g
)

create table Comenzi(
	cid int primary key,
	nrTelefon varchar(10),   -- nr de telefon al clientului
	deadline Date     -- data la care se va face livrarea comenzii
)


-- "junction table"
create table SortimenteComenzi(
	sid int,
	cid int,
	constraint sortimente_comenzi_pk primary key (sid, cid),
	constraint fk_sortimente foreign key (sid) references Sortimente(sid),
	constraint fk_comenzi foreign key (cid) references Comenzi(cid)
)