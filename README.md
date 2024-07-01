Back por consola

1. Abrir el archivo por consola de cmd o powershell, abres la carpeta donde esta alojada y pones dotnet run y ya podreas ejecutar los siguientes ejemplos.

post @Alfonso Hola Mundo
post @Alfonso Adiós mundo cruel
post @Ivan Hoy puede ser un gran dia
post @Ivan Para casa ya, media jornada, 12h
follow @Alicia @Ivan
follow @Alicia @Fonso
follow @Alicia @Ivan
follow @Alicia @Alfonso
wall @Alicia

2. Para aplicar las pruebas unitarias es solo ejecutar "dotnet test" 

Back Web Api

1. Intalar .netcore 8
2. Usar net core 8: Visual studio 2022
3. Descargar el proyecto
4. Crear la base de datos si se va a probar en web (Microsoft sql server)
	- Back/DataBase o usar migracion
5. Cambiar direccionamiento de sql si se va a trabajar web
	-"ConnectionStrings": {
  	"Connection": ".\\SQLEXPRESS; Database=SocialNetworkDB;Trusted_Connection=True;TrustServerCertificate=True;" 
	organizar direccion en appsettings.json en caso de tener problemas en la parte especifica de .\\SQLEXPRESS aqui iria tu direccion 
6. ya se podria ejecutar utilisando la interface o el comando dotner run o si se va a realizar cambios en codigo dotnet watch run

Front

1. Tener instalado Node.js
2. Abrir la carpeta en Visual Estudio Code
3. Abrir la carpeta en la terminal para ejecutar npm install o npm i para instalar los modulos
4. Ejecutar el front npm run dev que tambien los comandos de produccion y se lanza igual 


