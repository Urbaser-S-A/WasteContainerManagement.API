-- ============================================================================
-- Script 001: Initial schema creation for Waste Container Management API
-- Database: PostgreSQL
-- EF Core Provider: Npgsql.EntityFrameworkCore.PostgreSQL 10.0.0
-- Date: 2026-03-06
-- ============================================================================

-- Create EF Core migrations history table
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Check if migration has already been applied
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20260306000000_InitialCreate'
    ) THEN
        RAISE NOTICE 'Migration 20260306000000_InitialCreate already applied. Skipping.';
        RETURN;
    END IF;

    -- ========================================================================
    -- Table: WasteTypes
    -- ========================================================================
    CREATE TABLE IF NOT EXISTS "WasteTypes" (
        "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
        "Name" varchar(100) NOT NULL,
        "Description" varchar(500) NULL,
        "ColorCode" varchar(7) NULL,
        "IsActive" boolean NOT NULL DEFAULT true,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
        "UpdatedAt" timestamp with time zone NULL,
        CONSTRAINT "PK_WasteTypes" PRIMARY KEY ("Id")
    );

    CREATE UNIQUE INDEX IF NOT EXISTS "IX_WasteTypes_Name" ON "WasteTypes" ("Name");

    COMMENT ON TABLE "WasteTypes" IS 'Catalog of waste types (organic, paper, plastic, glass, etc.)';
    COMMENT ON COLUMN "WasteTypes"."ColorCode" IS 'Hex color code for UI representation (e.g., #00FF00)';

    -- ========================================================================
    -- Table: Zones
    -- ========================================================================
    CREATE TABLE IF NOT EXISTS "Zones" (
        "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
        "Name" varchar(150) NOT NULL,
        "District" varchar(150) NULL,
        "City" varchar(150) NULL,
        "IsActive" boolean NOT NULL DEFAULT true,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
        "UpdatedAt" timestamp with time zone NULL,
        CONSTRAINT "PK_Zones" PRIMARY KEY ("Id")
    );

    CREATE UNIQUE INDEX IF NOT EXISTS "IX_Zones_Name" ON "Zones" ("Name");

    COMMENT ON TABLE "Zones" IS 'Geographic zones for container deployment';

    -- ========================================================================
    -- Table: Containers
    -- ========================================================================
    CREATE TABLE IF NOT EXISTS "Containers" (
        "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
        "Code" varchar(50) NOT NULL,
        "WasteTypeId" uuid NOT NULL,
        "ZoneId" uuid NOT NULL,
        "Latitude" double precision NOT NULL DEFAULT 0,
        "Longitude" double precision NOT NULL DEFAULT 0,
        "Address" varchar(300) NULL,
        "CapacityLiters" integer NOT NULL DEFAULT 0,
        "Status" varchar(20) NOT NULL DEFAULT 'Active',
        "InstallationDate" timestamp with time zone NULL,
        "LastCollectionDate" timestamp with time zone NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
        "UpdatedAt" timestamp with time zone NULL,
        CONSTRAINT "PK_Containers" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Containers_WasteTypes" FOREIGN KEY ("WasteTypeId")
            REFERENCES "WasteTypes" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Containers_Zones" FOREIGN KEY ("ZoneId")
            REFERENCES "Zones" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "CK_Containers_Status" CHECK (
            "Status" IN ('Active', 'Inactive', 'Maintenance', 'Full')
        )
    );

    CREATE UNIQUE INDEX IF NOT EXISTS "IX_Containers_Code" ON "Containers" ("Code");
    CREATE INDEX IF NOT EXISTS "IX_Containers_WasteTypeId" ON "Containers" ("WasteTypeId");
    CREATE INDEX IF NOT EXISTS "IX_Containers_ZoneId" ON "Containers" ("ZoneId");
    CREATE INDEX IF NOT EXISTS "IX_Containers_Status" ON "Containers" ("Status");

    COMMENT ON TABLE "Containers" IS 'Physical waste containers deployed across zones';
    COMMENT ON COLUMN "Containers"."Code" IS 'Unique container identifier code (e.g., CTN-001-ORG)';
    COMMENT ON COLUMN "Containers"."Status" IS 'Container status: Active, Inactive, Maintenance, Full';

    -- ========================================================================
    -- Table: Incidents
    -- ========================================================================
    CREATE TABLE IF NOT EXISTS "Incidents" (
        "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
        "ContainerId" uuid NOT NULL,
        "Type" varchar(30) NOT NULL,
        "Description" varchar(1000) NULL,
        "Status" varchar(20) NOT NULL DEFAULT 'Open',
        "Priority" varchar(20) NOT NULL DEFAULT 'Medium',
        "ReportedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
        "ResolvedAt" timestamp with time zone NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
        "UpdatedAt" timestamp with time zone NULL,
        CONSTRAINT "PK_Incidents" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Incidents_Containers" FOREIGN KEY ("ContainerId")
            REFERENCES "Containers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "CK_Incidents_Type" CHECK (
            "Type" IN ('Overflow', 'Damage', 'Vandalism', 'CleaningRequired', 'Other')
        ),
        CONSTRAINT "CK_Incidents_Status" CHECK (
            "Status" IN ('Open', 'InProgress', 'Resolved', 'Closed')
        ),
        CONSTRAINT "CK_Incidents_Priority" CHECK (
            "Priority" IN ('Low', 'Medium', 'High', 'Critical')
        )
    );

    CREATE INDEX IF NOT EXISTS "IX_Incidents_ContainerId" ON "Incidents" ("ContainerId");
    CREATE INDEX IF NOT EXISTS "IX_Incidents_Status" ON "Incidents" ("Status");
    CREATE INDEX IF NOT EXISTS "IX_Incidents_Priority" ON "Incidents" ("Priority");
    CREATE INDEX IF NOT EXISTS "IX_Incidents_ReportedAt" ON "Incidents" ("ReportedAt");

    COMMENT ON TABLE "Incidents" IS 'Incidents reported on containers (overflow, damage, etc.)';
    COMMENT ON COLUMN "Incidents"."Type" IS 'Incident type: Overflow, Damage, Vandalism, CleaningRequired, Other';
    COMMENT ON COLUMN "Incidents"."Status" IS 'Incident status: Open, InProgress, Resolved, Closed';
    COMMENT ON COLUMN "Incidents"."Priority" IS 'Incident priority: Low, Medium, High, Critical';

    -- ========================================================================
    -- Record migration
    -- ========================================================================
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260306000000_InitialCreate', '10.0.0')
    ON CONFLICT ("MigrationId") DO NOTHING;

    RAISE NOTICE 'Migration 20260306000000_InitialCreate applied successfully.';

END $$;
