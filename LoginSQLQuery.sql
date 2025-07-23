GO
CREATE DATABASE DB_ACCESO
GO
USE DB_ACCESO

GO
CREATE TABLE USUARIO(
IDUsuario int primary key identity(1,1),
Correo Varchar(100),
Clave Varchar(500)
)

GO
CREATE PROC SP_RegistrarUsuario(
@Correo Varchar(100),
@Clave Varchar(500),
@Registrado bit output,
@Mensaje varchar (100) output
)
as
begin

	if(not exists(select * from USUARIO where Correo = @Correo))
	begin
		Insert into USUARIO(Correo,Clave) Values(@Correo,@Clave)
		Set @Registrado = 1
		Set @Mensaje = 'Usuario registrado correctamente'
	end
	else
	begin
		set @Registrado = 0
		set	@Mensaje = 'Este correo ya esta registrado'
	end
end

GO
CREATE PROC SP_ValidarUsuario(
@Correo Varchar(100),
@Clave Varchar(500)
)
as
begin
	If(exists(Select * from USUARIO where Correo = @Correo and Clave = @Clave))
		Select IdUsuario from USUARIO where Correo = @Correo and Clave = @Clave
	else
		Select 
		'0'
	end

	/*PROBANDO PROCEDIMIENTO REGISTRAR USUARIO*/
GO
Declare @registrado bit, @mensaje Varchar(100)

exec SP_RegistrarUsuario 'Jescalante@gmail.com', 'Jaime1234', @registrado output, @mensaje output

Select @registrado
Select @mensaje

	/*PROBANDO PROCEDIMIENTO VALIDAR USUARIO*/
GO
exec SP_ValidarUsuario 'Jescalante@gmail.com', 'Jaime1234'
