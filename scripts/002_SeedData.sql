-- ============================================================================
-- Script 002: Seed data for Waste Container Management API (POC)
-- Database: PostgreSQL
-- Date: 2026-03-13
-- Updated: 2026-03-30 (enriched data for comprehensive endpoint coverage)
-- ============================================================================

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260313000000_SeedData'
    ) THEN
        RAISE NOTICE 'Migration 20260313000000_SeedData already applied. Skipping.';
        RETURN;
    END IF;

    -- ========================================================================
    -- Seed: WasteTypes (6 tipos - 5 activos + 1 inactivo)
    -- ========================================================================
    INSERT INTO "WasteTypes" ("Id", "Name", "Description", "ColorCode", "IsActive", "CreatedAt")
    VALUES
        ('a1000000-0000-0000-0000-000000000001', 'Orgánica', 'Residuos orgánicos y biodegradables: restos de comida, poda, etc.', '#8B4513', true, '2026-01-15T10:00:00Z'),
        ('a1000000-0000-0000-0000-000000000002', 'Papel y Cartón', 'Papel, cartón, periódicos, revistas y envases de cartón', '#1E90FF', true, '2026-01-15T10:05:00Z'),
        ('a1000000-0000-0000-0000-000000000003', 'Envases', 'Envases de plástico, latas, briks y envases metálicos', '#FFD700', true, '2026-01-15T10:10:00Z'),
        ('a1000000-0000-0000-0000-000000000004', 'Vidrio', 'Botellas, tarros y envases de vidrio', '#228B22', true, '2026-01-15T10:15:00Z'),
        ('a1000000-0000-0000-0000-000000000005', 'Resto', 'Residuos no reciclables que no pertenecen a otras fracciones', '#708090', true, '2026-01-15T10:20:00Z'),
        ('a1000000-0000-0000-0000-000000000006', 'Textil', 'Ropa, calzado y textiles usados. Fracción en fase piloto, no operativa actualmente.', '#9932CC', false, '2026-01-15T10:25:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Zones (6 distritos de Madrid - 5 activas + 1 inactiva)
    -- ========================================================================
    INSERT INTO "Zones" ("Id", "Name", "District", "City", "IsActive", "CreatedAt")
    VALUES
        ('b2000000-0000-0000-0000-000000000001', 'Zona Norte-A', 'Chamartín', 'Madrid', true, '2026-01-16T09:00:00Z'),
        ('b2000000-0000-0000-0000-000000000002', 'Zona Sur-B', 'Usera', 'Madrid', true, '2026-01-16T09:15:00Z'),
        ('b2000000-0000-0000-0000-000000000003', 'Zona Centro-C', 'Centro', 'Madrid', true, '2026-01-16T09:30:00Z'),
        ('b2000000-0000-0000-0000-000000000004', 'Zona Este-D', 'Salamanca', 'Madrid', true, '2026-01-16T09:45:00Z'),
        ('b2000000-0000-0000-0000-000000000005', 'Zona Suroeste-E', 'Latina', 'Madrid', true, '2026-01-16T10:00:00Z'),
        ('b2000000-0000-0000-0000-000000000006', 'Zona Noreste-F', 'Hortaleza', 'Madrid', false, '2026-01-16T10:15:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Containers (20 contenedores)
    -- Distribución: todas las zonas activas, todos los tipos activos, todos los estados
    -- ========================================================================
    INSERT INTO "Containers" ("Id", "Code", "WasteTypeId", "ZoneId", "Latitude", "Longitude", "Address", "CapacityLiters", "Status", "InstallationDate", "LastCollectionDate", "CreatedAt")
    VALUES
        -- Zona Norte-A (Chamartín) - 4 contenedores
        ('c3000000-0000-0000-0000-000000000001', 'CNT-2026-001', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000001',
         40.4625, -3.6773, 'Paseo de la Castellana 140, Madrid', 1100, 'Active', '2026-01-20T00:00:00Z', '2026-03-28T07:30:00Z', '2026-01-20T08:00:00Z'),
        ('c3000000-0000-0000-0000-000000000002', 'CNT-2026-002', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000001',
         40.4630, -3.6780, 'Paseo de la Castellana 142, Madrid', 800, 'Active', '2026-01-20T00:00:00Z', '2026-03-27T06:45:00Z', '2026-01-20T08:15:00Z'),
        ('c3000000-0000-0000-0000-000000000003', 'CNT-2026-003', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000001',
         40.4628, -3.6775, 'Paseo de la Castellana 140, Madrid', 600, 'Maintenance', '2026-01-20T00:00:00Z', '2026-03-15T08:00:00Z', '2026-01-20T08:30:00Z'),
        ('c3000000-0000-0000-0000-000000000004', 'CNT-2026-004', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000001',
         40.4632, -3.6770, 'Calle Agustín de Foxá 25, Madrid', 800, 'Active', '2026-01-22T00:00:00Z', '2026-03-28T06:00:00Z', '2026-01-22T09:00:00Z'),

        -- Zona Sur-B (Usera) - 4 contenedores
        ('c3000000-0000-0000-0000-000000000005', 'CNT-2026-005', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000002',
         40.3850, -3.7100, 'Avenida de Rafaela Ybarra 12, Madrid', 1100, 'Full', '2026-02-01T00:00:00Z', '2026-03-25T07:00:00Z', '2026-02-01T09:00:00Z'),
        ('c3000000-0000-0000-0000-000000000006', 'CNT-2026-006', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000002',
         40.3855, -3.7105, 'Avenida de Rafaela Ybarra 14, Madrid', 800, 'Active', '2026-02-01T00:00:00Z', '2026-03-28T06:30:00Z', '2026-02-01T09:15:00Z'),
        ('c3000000-0000-0000-0000-000000000007', 'CNT-2026-007', 'a1000000-0000-0000-0000-000000000005', 'b2000000-0000-0000-0000-000000000002',
         40.3848, -3.7098, 'Calle Marcelo Usera 40, Madrid', 1100, 'Active', '2026-02-01T00:00:00Z', '2026-03-27T07:15:00Z', '2026-02-01T09:30:00Z'),
        ('c3000000-0000-0000-0000-000000000008', 'CNT-2026-008', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000002',
         40.3852, -3.7108, 'Calle Marcelo Usera 42, Madrid', 800, 'Inactive', '2026-02-01T00:00:00Z', NULL, '2026-02-01T09:45:00Z'),

        -- Zona Centro-C (Centro) - 4 contenedores
        ('c3000000-0000-0000-0000-000000000009', 'CNT-2026-009', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000003',
         40.4168, -3.7038, 'Calle Gran Vía 28, Madrid', 1100, 'Active', '2026-01-25T00:00:00Z', '2026-03-28T07:00:00Z', '2026-01-25T10:00:00Z'),
        ('c3000000-0000-0000-0000-000000000010', 'CNT-2026-010', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000003',
         40.4170, -3.7040, 'Calle Gran Vía 30, Madrid', 800, 'Maintenance', '2026-01-25T00:00:00Z', '2026-03-20T07:15:00Z', '2026-01-25T10:15:00Z'),
        ('c3000000-0000-0000-0000-000000000011', 'CNT-2026-011', 'a1000000-0000-0000-0000-000000000005', 'b2000000-0000-0000-0000-000000000003',
         40.4165, -3.7035, 'Calle Gran Vía 26, Madrid', 1100, 'Active', '2026-01-25T00:00:00Z', '2026-03-28T07:30:00Z', '2026-01-25T10:30:00Z'),
        ('c3000000-0000-0000-0000-000000000012', 'CNT-2026-012', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000003',
         40.4172, -3.7042, 'Plaza de Callao 3, Madrid', 600, 'Full', '2026-01-25T00:00:00Z', '2026-03-26T08:00:00Z', '2026-01-25T10:45:00Z'),

        -- Zona Este-D (Salamanca) - 4 contenedores
        ('c3000000-0000-0000-0000-000000000013', 'CNT-2026-013', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000004',
         40.4280, -3.6820, 'Calle Serrano 45, Madrid', 800, 'Active', '2026-02-10T00:00:00Z', '2026-03-27T06:50:00Z', '2026-02-10T11:00:00Z'),
        ('c3000000-0000-0000-0000-000000000014', 'CNT-2026-014', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000004',
         40.4285, -3.6825, 'Calle Serrano 47, Madrid', 600, 'Active', '2026-02-10T00:00:00Z', '2026-03-28T07:00:00Z', '2026-02-10T11:15:00Z'),
        ('c3000000-0000-0000-0000-000000000015', 'CNT-2026-015', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000004',
         40.4278, -3.6818, 'Calle Goya 12, Madrid', 1100, 'Active', '2026-02-10T00:00:00Z', '2026-03-28T06:30:00Z', '2026-02-10T11:30:00Z'),
        ('c3000000-0000-0000-0000-000000000016', 'CNT-2026-016', 'a1000000-0000-0000-0000-000000000005', 'b2000000-0000-0000-0000-000000000004',
         40.4282, -3.6828, 'Calle Goya 14, Madrid', 1100, 'Inactive', '2026-02-10T00:00:00Z', NULL, '2026-02-10T11:45:00Z'),

        -- Zona Suroeste-E (Latina) - 4 contenedores
        ('c3000000-0000-0000-0000-000000000017', 'CNT-2026-017', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000005',
         40.4020, -3.7450, 'Paseo de Extremadura 100, Madrid', 1100, 'Active', '2026-02-15T00:00:00Z', '2026-03-28T06:00:00Z', '2026-02-15T08:00:00Z'),
        ('c3000000-0000-0000-0000-000000000018', 'CNT-2026-018', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000005',
         40.4025, -3.7455, 'Paseo de Extremadura 102, Madrid', 800, 'Active', '2026-02-15T00:00:00Z', '2026-03-27T06:15:00Z', '2026-02-15T08:15:00Z'),
        ('c3000000-0000-0000-0000-000000000019', 'CNT-2026-019', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000005',
         40.4018, -3.7448, 'Calle Camarena 50, Madrid', 800, 'Full', '2026-02-15T00:00:00Z', '2026-03-26T07:30:00Z', '2026-02-15T08:30:00Z'),
        ('c3000000-0000-0000-0000-000000000020', 'CNT-2026-020', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000005',
         40.4022, -3.7452, 'Calle Camarena 52, Madrid', 600, 'Active', '2026-02-15T00:00:00Z', '2026-03-28T07:00:00Z', '2026-02-15T08:45:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Incidents (15 incidencias)
    -- Cobertura: todos los tipos (5), todos los estados (4), todas las prioridades (4)
    -- Rango de fechas: enero 2026 - marzo 2026
    -- ========================================================================
    INSERT INTO "Incidents" ("Id", "ContainerId", "Type", "Description", "Status", "Priority", "ReportedAt", "ResolvedAt", "CreatedAt")
    VALUES
        -- Open incidents (3) - distintos tipos y prioridades
        ('d4000000-0000-0000-0000-000000000001', 'c3000000-0000-0000-0000-000000000005',
         'Overflow', 'Contenedor desbordado tras el fin de semana. Residuos orgánicos en la acera.', 'Open', 'High',
         '2026-03-24T08:15:00Z', NULL, '2026-03-24T08:15:00Z'),

        ('d4000000-0000-0000-0000-000000000002', 'c3000000-0000-0000-0000-000000000009',
         'CleaningRequired', 'Derrame de líquidos alrededor del contenedor. Malos olores.', 'Open', 'Medium',
         '2026-03-26T09:45:00Z', NULL, '2026-03-26T09:45:00Z'),

        ('d4000000-0000-0000-0000-000000000003', 'c3000000-0000-0000-0000-000000000019',
         'Overflow', 'Contenedor de envases lleno por acumulación en zona comercial.', 'Open', 'Critical',
         '2026-03-28T10:00:00Z', NULL, '2026-03-28T10:00:00Z'),

        -- InProgress incidents (3) - distintos tipos y prioridades
        ('d4000000-0000-0000-0000-000000000004', 'c3000000-0000-0000-0000-000000000013',
         'Vandalism', 'Grafitis y pegatinas cubriendo el contenedor. Señalización de reciclaje ilegible.', 'InProgress', 'Low',
         '2026-03-20T11:00:00Z', NULL, '2026-03-20T11:00:00Z'),

        ('d4000000-0000-0000-0000-000000000005', 'c3000000-0000-0000-0000-000000000012',
         'Damage', 'Pedal de apertura roto, mecanismo atascado. Se requiere pieza de repuesto.', 'InProgress', 'High',
         '2026-03-22T14:30:00Z', NULL, '2026-03-22T14:30:00Z'),

        ('d4000000-0000-0000-0000-000000000006', 'c3000000-0000-0000-0000-000000000017',
         'Other', 'Contenedor desplazado de su ubicación original por obras en la vía pública.', 'InProgress', 'Medium',
         '2026-03-23T16:20:00Z', NULL, '2026-03-23T16:20:00Z'),

        -- Resolved incidents (5) - distintos tipos y prioridades
        ('d4000000-0000-0000-0000-000000000007', 'c3000000-0000-0000-0000-000000000003',
         'Damage', 'Mecanismo de apertura de tapa roto, no cierra correctamente.', 'Resolved', 'Medium',
         '2026-03-01T14:30:00Z', '2026-03-05T10:00:00Z', '2026-03-01T14:30:00Z'),

        ('d4000000-0000-0000-0000-000000000008', 'c3000000-0000-0000-0000-000000000006',
         'CleaningRequired', 'Acumulación de residuos sueltos alrededor de la base del contenedor.', 'Resolved', 'Low',
         '2026-02-20T09:00:00Z', '2026-02-21T11:30:00Z', '2026-02-20T09:00:00Z'),

        ('d4000000-0000-0000-0000-000000000009', 'c3000000-0000-0000-0000-000000000015',
         'Overflow', 'Contenedor orgánico lleno tras festividad local. Recogida extra solicitada.', 'Resolved', 'High',
         '2026-03-10T07:30:00Z', '2026-03-10T14:00:00Z', '2026-03-10T07:30:00Z'),

        ('d4000000-0000-0000-0000-000000000010', 'c3000000-0000-0000-0000-000000000018',
         'Vandalism', 'Contenedor volcado intencionadamente durante la noche.', 'Resolved', 'Critical',
         '2026-02-28T06:00:00Z', '2026-02-28T12:00:00Z', '2026-02-28T06:00:00Z'),

        ('d4000000-0000-0000-0000-000000000011', 'c3000000-0000-0000-0000-000000000001',
         'Other', 'Señalización de tipo de residuo incorrecta. Etiqueta de orgánica despegada.', 'Resolved', 'Low',
         '2026-03-05T13:00:00Z', '2026-03-07T09:00:00Z', '2026-03-05T13:00:00Z'),

        -- Closed incidents (4) - distintos tipos y prioridades
        ('d4000000-0000-0000-0000-000000000012', 'c3000000-0000-0000-0000-000000000001',
         'Overflow', 'Contenedor lleno por acumulación tras festivo local.', 'Closed', 'High',
         '2026-02-15T07:30:00Z', '2026-02-15T14:00:00Z', '2026-02-15T07:30:00Z'),

        ('d4000000-0000-0000-0000-000000000013', 'c3000000-0000-0000-0000-000000000010',
         'Damage', 'Rueda de transporte bloqueada. Contenedor inamovible para recogida.', 'Closed', 'Critical',
         '2026-01-28T10:00:00Z', '2026-01-30T15:00:00Z', '2026-01-28T10:00:00Z'),

        ('d4000000-0000-0000-0000-000000000014', 'c3000000-0000-0000-0000-000000000007',
         'CleaningRequired', 'Vertido de aceite de cocina en el interior del contenedor de resto.', 'Closed', 'Medium',
         '2026-02-05T08:30:00Z', '2026-02-06T10:00:00Z', '2026-02-05T08:30:00Z'),

        ('d4000000-0000-0000-0000-000000000015', 'c3000000-0000-0000-0000-000000000020',
         'Vandalism', 'Cerradura forzada y contenido esparcido por la acera.', 'Closed', 'High',
         '2026-02-12T22:00:00Z', '2026-02-13T11:00:00Z', '2026-02-12T22:00:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Record migration
    -- ========================================================================
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260313000000_SeedData', '10.0.0')
    ON CONFLICT ("MigrationId") DO NOTHING;

    RAISE NOTICE 'Migration 20260313000000_SeedData applied successfully.';

END $$;
