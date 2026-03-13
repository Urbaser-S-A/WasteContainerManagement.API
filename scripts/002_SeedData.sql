-- ============================================================================
-- Script 002: Seed data for Waste Container Management API (POC)
-- Database: PostgreSQL
-- Date: 2026-03-13
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
    -- Seed: WasteTypes (5 tipos del sistema español de reciclaje)
    -- ========================================================================
    INSERT INTO "WasteTypes" ("Id", "Name", "Description", "ColorCode", "IsActive", "CreatedAt")
    VALUES
        ('a1000000-0000-0000-0000-000000000001', 'Orgánica', 'Residuos orgánicos y biodegradables: restos de comida, poda, etc.', '#8B4513', true, '2026-01-15T10:00:00Z'),
        ('a1000000-0000-0000-0000-000000000002', 'Papel y Cartón', 'Papel, cartón, periódicos, revistas y envases de cartón', '#1E90FF', true, '2026-01-15T10:05:00Z'),
        ('a1000000-0000-0000-0000-000000000003', 'Envases', 'Envases de plástico, latas, briks y envases metálicos', '#FFD700', true, '2026-01-15T10:10:00Z'),
        ('a1000000-0000-0000-0000-000000000004', 'Vidrio', 'Botellas, tarros y envases de vidrio', '#228B22', true, '2026-01-15T10:15:00Z'),
        ('a1000000-0000-0000-0000-000000000005', 'Resto', 'Residuos no reciclables que no pertenecen a otras fracciones', '#708090', true, '2026-01-15T10:20:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Zones (4 distritos de Madrid)
    -- ========================================================================
    INSERT INTO "Zones" ("Id", "Name", "District", "City", "IsActive", "CreatedAt")
    VALUES
        ('b2000000-0000-0000-0000-000000000001', 'Zona Norte-A', 'Chamartín', 'Madrid', true, '2026-01-16T09:00:00Z'),
        ('b2000000-0000-0000-0000-000000000002', 'Zona Sur-B', 'Usera', 'Madrid', true, '2026-01-16T09:15:00Z'),
        ('b2000000-0000-0000-0000-000000000003', 'Zona Centro-C', 'Centro', 'Madrid', true, '2026-01-16T09:30:00Z'),
        ('b2000000-0000-0000-0000-000000000004', 'Zona Este-D', 'Salamanca', 'Madrid', true, '2026-01-16T09:45:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Containers (10 contenedores repartidos por zonas)
    -- ========================================================================
    INSERT INTO "Containers" ("Id", "Code", "WasteTypeId", "ZoneId", "Latitude", "Longitude", "Address", "CapacityLiters", "Status", "InstallationDate", "LastCollectionDate", "CreatedAt")
    VALUES
        -- Zona Norte-A (Chamartín)
        ('c3000000-0000-0000-0000-000000000001', 'CNT-2026-001', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000001',
         40.4625, -3.6773, 'Paseo de la Castellana 140, Madrid', 1100, 'Active', '2026-01-20T00:00:00Z', '2026-03-12T07:30:00Z', '2026-01-20T08:00:00Z'),
        ('c3000000-0000-0000-0000-000000000002', 'CNT-2026-002', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000001',
         40.4630, -3.6780, 'Paseo de la Castellana 142, Madrid', 800, 'Active', '2026-01-20T00:00:00Z', '2026-03-11T06:45:00Z', '2026-01-20T08:15:00Z'),
        ('c3000000-0000-0000-0000-000000000003', 'CNT-2026-003', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000001',
         40.4628, -3.6775, 'Paseo de la Castellana 140, Madrid', 600, 'Active', '2026-01-20T00:00:00Z', '2026-03-10T08:00:00Z', '2026-01-20T08:30:00Z'),

        -- Zona Sur-B (Usera)
        ('c3000000-0000-0000-0000-000000000004', 'CNT-2026-004', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000002',
         40.3850, -3.7100, 'Avenida de Rafaela Ybarra 12, Madrid', 1100, 'Full', '2026-02-01T00:00:00Z', '2026-03-10T07:00:00Z', '2026-02-01T09:00:00Z'),
        ('c3000000-0000-0000-0000-000000000005', 'CNT-2026-005', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000002',
         40.3855, -3.7105, 'Avenida de Rafaela Ybarra 14, Madrid', 800, 'Active', '2026-02-01T00:00:00Z', '2026-03-12T06:30:00Z', '2026-02-01T09:15:00Z'),

        -- Zona Centro-C (Centro)
        ('c3000000-0000-0000-0000-000000000006', 'CNT-2026-006', 'a1000000-0000-0000-0000-000000000001', 'b2000000-0000-0000-0000-000000000003',
         40.4168, -3.7038, 'Calle Gran Vía 28, Madrid', 1100, 'Active', '2026-01-25T00:00:00Z', '2026-03-12T07:00:00Z', '2026-01-25T10:00:00Z'),
        ('c3000000-0000-0000-0000-000000000007', 'CNT-2026-007', 'a1000000-0000-0000-0000-000000000003', 'b2000000-0000-0000-0000-000000000003',
         40.4170, -3.7040, 'Calle Gran Vía 30, Madrid', 800, 'Maintenance', '2026-01-25T00:00:00Z', '2026-03-08T07:15:00Z', '2026-01-25T10:15:00Z'),
        ('c3000000-0000-0000-0000-000000000008', 'CNT-2026-008', 'a1000000-0000-0000-0000-000000000005', 'b2000000-0000-0000-0000-000000000003',
         40.4165, -3.7035, 'Calle Gran Vía 26, Madrid', 1100, 'Active', '2026-01-25T00:00:00Z', '2026-03-12T07:30:00Z', '2026-01-25T10:30:00Z'),

        -- Zona Este-D (Salamanca)
        ('c3000000-0000-0000-0000-000000000009', 'CNT-2026-009', 'a1000000-0000-0000-0000-000000000002', 'b2000000-0000-0000-0000-000000000004',
         40.4280, -3.6820, 'Calle Serrano 45, Madrid', 800, 'Active', '2026-02-10T00:00:00Z', '2026-03-11T06:50:00Z', '2026-02-10T11:00:00Z'),
        ('c3000000-0000-0000-0000-000000000010', 'CNT-2026-010', 'a1000000-0000-0000-0000-000000000004', 'b2000000-0000-0000-0000-000000000004',
         40.4285, -3.6825, 'Calle Serrano 47, Madrid', 600, 'Inactive', '2026-02-10T00:00:00Z', NULL, '2026-02-10T11:15:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Seed: Incidents (6 incidencias variadas)
    -- ========================================================================
    INSERT INTO "Incidents" ("Id", "ContainerId", "Type", "Description", "Status", "Priority", "ReportedAt", "ResolvedAt", "CreatedAt")
    VALUES
        -- Desbordamiento en Usera (abierta, alta prioridad)
        ('d4000000-0000-0000-0000-000000000001', 'c3000000-0000-0000-0000-000000000004',
         'Overflow', 'Contenedor desbordado tras el fin de semana. Residuos orgánicos en la acera.', 'Open', 'High',
         '2026-03-10T08:15:00Z', NULL, '2026-03-10T08:15:00Z'),

        -- Daño en mecanismo de tapa en Centro (resuelta)
        ('d4000000-0000-0000-0000-000000000002', 'c3000000-0000-0000-0000-000000000007',
         'Damage', 'Mecanismo de apertura de tapa roto, no cierra correctamente.', 'Resolved', 'Medium',
         '2026-03-01T14:30:00Z', '2026-03-05T10:00:00Z', '2026-03-01T14:30:00Z'),

        -- Vandalismo en Salamanca (en progreso)
        ('d4000000-0000-0000-0000-000000000003', 'c3000000-0000-0000-0000-000000000010',
         'Vandalism', 'Grafitis y pegatinas cubriendo el contenedor. Señalización de reciclaje ilegible.', 'InProgress', 'Low',
         '2026-03-08T11:00:00Z', NULL, '2026-03-08T11:00:00Z'),

        -- Limpieza requerida en Centro (abierta)
        ('d4000000-0000-0000-0000-000000000004', 'c3000000-0000-0000-0000-000000000006',
         'CleaningRequired', 'Derrame de líquidos alrededor del contenedor. Malos olores.', 'Open', 'Medium',
         '2026-03-12T09:45:00Z', NULL, '2026-03-12T09:45:00Z'),

        -- Desbordamiento en Chamartín (cerrada)
        ('d4000000-0000-0000-0000-000000000005', 'c3000000-0000-0000-0000-000000000001',
         'Overflow', 'Contenedor lleno por acumulación tras festivo local.', 'Closed', 'High',
         '2026-02-15T07:30:00Z', '2026-02-15T14:00:00Z', '2026-02-15T07:30:00Z'),

        -- Otro tipo en Usera (resuelta)
        ('d4000000-0000-0000-0000-000000000006', 'c3000000-0000-0000-0000-000000000005',
         'Other', 'Contenedor desplazado de su ubicación original por obras en la vía pública.', 'Resolved', 'Critical',
         '2026-03-05T16:20:00Z', '2026-03-06T09:00:00Z', '2026-03-05T16:20:00Z')
    ON CONFLICT ("Id") DO NOTHING;

    -- ========================================================================
    -- Record migration
    -- ========================================================================
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260313000000_SeedData', '10.0.0')
    ON CONFLICT ("MigrationId") DO NOTHING;

    RAISE NOTICE 'Migration 20260313000000_SeedData applied successfully.';

END $$;
