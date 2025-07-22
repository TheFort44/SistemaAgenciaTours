CREATE DATABASE SistemaAgenciaTours;
GO

USE SistemaAgenciaTours;
GO

GO
CREATE TABLE Pais (
    PaisID INT PRIMARY KEY IDENTITY(1,1),
    NombrePais VARCHAR(100) NOT NULL
);

GO
CREATE TABLE Destino (
    DestinoID INT PRIMARY KEY IDENTITY(1,1),
    PaisID INT NOT NULL,
    NombreDestino VARCHAR(100) NOT NULL,
    DuracionHoras INT NOT NULL, -- Duración total en horas

    CONSTRAINT FK_Destino_Pais FOREIGN KEY (PaisID)
        REFERENCES Pais(PaisID)
);

GO
CREATE TABLE Tour (
    TourID INT PRIMARY KEY IDENTITY(1,1),
    NombreTour VARCHAR(150) NOT NULL,
    PaisID INT NOT NULL,
    DestinoID INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,

    ITBIS AS (Precio * 0.18) PERSISTED,

    CONSTRAINT FK_Tour_Pais FOREIGN KEY (PaisID) REFERENCES Pais(PaisID),
    CONSTRAINT FK_Tour_Destino FOREIGN KEY (DestinoID) REFERENCES Destino(DestinoID)
);

GO
CREATE VIEW Vista_TourConEstado AS
SELECT 
    T.TourID,
    T.NombreTour,
    T.PaisID,
    P.NombrePais,
    T.DestinoID,
    D.NombreDestino,
    T.Fecha,
    T.Hora,
    T.Precio,
    T.ITBIS,
    D.DuracionHoras,
    DATEADD(HOUR, D.DuracionHoras, CAST(T.Fecha AS DATETIME) + CAST(T.Hora AS DATETIME)) AS FechaHoraFin,
    CASE 
        WHEN DATEADD(HOUR, D.DuracionHoras, CAST(T.Fecha AS DATETIME) + CAST(T.Hora AS DATETIME)) > GETDATE()
            THEN 'Vigente'
        ELSE 'Vencido'
    END AS Estado
FROM Tour T
INNER JOIN Destino D ON T.DestinoID = D.DestinoID
INNER JOIN Pais P ON T.PaisID = P.PaisID;

SELECT * FROM Vista_TourConEstado