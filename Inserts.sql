-- Insertar pa�ses
INSERT INTO Pais (NombrePais) VALUES
('Rep�blica Dominicana'),
('Estados Unidos'),
('Espa�a'),
('M�xico'),
('Canad�'),
('Francia'),
('Italia'),
('Brasil'),
('Argentina'),
('Chile');

-- Insertar destinos
INSERT INTO Destino (PaisID, NombreDestino, DuracionHoras) VALUES
(1, 'Punta Cana', 8),
(1, 'Santo Domingo', 6),
(2, 'Nueva York', 10),
(2, 'Los �ngeles', 9),
(3, 'Barcelona', 9),
(3, 'Madrid', 7),
(4, 'Canc�n', 7),
(5, 'Toronto', 8),
(6, 'Par�s', 10),
(7, 'Roma', 8);

-- Insertar tours
INSERT INTO Tour (NombreTour, PaisID, DestinoID, Fecha, Hora, Precio) VALUES
('Tour de Playa en Punta Cana', 1, 1, '2025-08-01', '08:00:00', 120.00),
('Tour Hist�rico en Santo Domingo', 1, 2, '2025-08-02', '09:00:00', 80.00),
('Explorando Nueva York', 2, 3, '2025-08-03', '10:00:00', 150.00),
('Ruta Cultural en Los �ngeles', 2, 4, '2025-08-04', '11:00:00', 140.00),
('Paseo por Barcelona', 3, 5, '2025-08-05', '12:00:00', 110.00),
('Madrid al M�ximo', 3, 6, '2025-08-06', '13:00:00', 100.00),
('Vacaciones en Canc�n', 4, 7, '2025-08-07', '14:00:00', 130.00),
('Tour por Toronto', 5, 8, '2025-08-08', '15:00:00', 125.00),
('Par�s rom�ntico', 6, 9, '2025-08-09', '16:00:00', 160.00),
('Historia y arte en Roma', 7, 10, '2025-08-10', '17:00:00', 140.00);
