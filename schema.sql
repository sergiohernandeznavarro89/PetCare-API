CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Email" character varying(255) NOT NULL,
    "PasswordHash" text NOT NULL,
    "DeviceToken" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Pets" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Species" character varying(50) NOT NULL,
    "Breed" character varying(100),
    "DateOfBirth" timestamp with time zone,
    "PhotoUrl" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Pets" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Pets_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Treatments" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" text,
    "Type" character varying(50) NOT NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "PK_Treatments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Treatments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PetTreatment" (
    "Id" uuid NOT NULL,
    "PetId" uuid NOT NULL,
    "TreatmentId" uuid NOT NULL,
    "FrequencyInDays" integer NOT NULL,
    "NextDueDate" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_PetTreatment" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PetTreatment_Pets_PetId" FOREIGN KEY ("PetId") REFERENCES "Pets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PetTreatment_Treatments_TreatmentId" FOREIGN KEY ("TreatmentId") REFERENCES "Treatments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PetTreatmentHistory" (
    "Id" uuid NOT NULL,
    "PetTreatmentId" uuid NOT NULL,
    "AppliedDate" timestamp with time zone NOT NULL,
    "Notes" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_PetTreatmentHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PetTreatmentHistory_PetTreatment_PetTreatmentId" FOREIGN KEY ("PetTreatmentId") REFERENCES "PetTreatment" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Pets_UserId" ON "Pets" ("UserId");

CREATE INDEX "IX_PetTreatment_PetId" ON "PetTreatment" ("PetId");

CREATE INDEX "IX_PetTreatment_TreatmentId" ON "PetTreatment" ("TreatmentId");

CREATE INDEX "IX_PetTreatmentHistory_PetTreatmentId" ON "PetTreatmentHistory" ("PetTreatmentId");

CREATE INDEX "IX_Treatments_UserId" ON "Treatments" ("UserId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260928162653_InitialCreate', '10.0.12');

COMMIT;

START TRANSACTION;
ALTER TABLE "PetTreatment" ADD "Notify" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260928163347_AddNotifyToPetTreatment', '10.0.12');

COMMIT;

START TRANSACTION;
DROP TABLE "PetTreatmentHistory";

DROP TABLE "PetTreatment";

DROP TABLE "Treatments";

CREATE TABLE "EventTypeDefinitions" (
    "Id" uuid NOT NULL,
    "UserId" text,
    "Name" text NOT NULL,
    "Icon" text,
    "Color" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UserId1" uuid,
    CONSTRAINT "PK_EventTypeDefinitions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EventTypeDefinitions_Users_UserId1" FOREIGN KEY ("UserId1") REFERENCES "Users" ("Id")
);

CREATE TABLE "HealthEvents" (
    "Id" uuid NOT NULL,
    "PetId" uuid NOT NULL,
    "EventTypeId" uuid NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "Title" text NOT NULL,
    "Notes" text,
    "Weight" numeric,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "HealthEventType" character varying(13) NOT NULL,
    "DrugName" text,
    "Dosage" text,
    "FrequencyValue" integer,
    "FrequencyUnit" integer,
    "StartDate" timestamp with time zone,
    "EndDate" timestamp with time zone,
    "VaccineName" text,
    "VaccineEvent_FrequencyValue" integer,
    "VaccineEvent_FrequencyUnit" integer,
    "VeterinarianName" text,
    "ClinicName" text,
    "Diagnosis" text,
    "IsHospitalization" boolean,
    "DischargeDate" timestamp with time zone,
    CONSTRAINT "PK_HealthEvents" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HealthEvents_EventTypeDefinitions_EventTypeId" FOREIGN KEY ("EventTypeId") REFERENCES "EventTypeDefinitions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HealthEvents_Pets_PetId" FOREIGN KEY ("PetId") REFERENCES "Pets" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_EventTypeDefinitions_UserId1" ON "EventTypeDefinitions" ("UserId1");

CREATE INDEX "IX_HealthEvents_EventTypeId" ON "HealthEvents" ("EventTypeId");

CREATE INDEX "IX_HealthEvents_PetId" ON "HealthEvents" ("PetId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260929154252_AddHealthEventsSystem', '10.0.12');

COMMIT;

START TRANSACTION;
INSERT INTO "EventTypeDefinitions" ("Id", "Color", "CreatedAt", "Icon", "Name", "UserId", "UserId1")
VALUES ('10000000-0000-0000-0000-000000000001', 'blue', TIMESTAMPTZ '2020-01-01T00:00:00Z', 'local_hospital', 'Visita Veterinaria', NULL, NULL);
INSERT INTO "EventTypeDefinitions" ("Id", "Color", "CreatedAt", "Icon", "Name", "UserId", "UserId1")
VALUES ('10000000-0000-0000-0000-000000000002', 'orange', TIMESTAMPTZ '2020-01-01T00:00:00Z', 'medication', 'Medicación', NULL, NULL);
INSERT INTO "EventTypeDefinitions" ("Id", "Color", "CreatedAt", "Icon", "Name", "UserId", "UserId1")
VALUES ('10000000-0000-0000-0000-000000000003', 'red', TIMESTAMPTZ '2020-01-01T00:00:00Z', 'vaccines', 'Vacunación', NULL, NULL);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260929170512_SeedDefaultEventTypesFix', '10.0.12');

COMMIT;

START TRANSACTION;
ALTER TABLE "HealthEvents" ADD "MedicationEvent_FrequencyUnit" integer;

ALTER TABLE "HealthEvents" ADD "MedicationEvent_FrequencyValue" integer;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260929231931_AddFrequencyToCustomEvent', '10.0.12');

COMMIT;

START TRANSACTION;
ALTER TABLE "HealthEvents" ADD "ParentVisitId" uuid;

CREATE INDEX "IX_HealthEvents_ParentVisitId" ON "HealthEvents" ("ParentVisitId");

ALTER TABLE "HealthEvents" ADD CONSTRAINT "FK_HealthEvents_HealthEvents_ParentVisitId" FOREIGN KEY ("ParentVisitId") REFERENCES "HealthEvents" ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260930073905_HealthEventsV2', '10.0.12');

COMMIT;

START TRANSACTION;
CREATE TABLE "HealthEventOccurrences" (
    "Id" uuid NOT NULL,
    "HealthEventId" uuid NOT NULL,
    "ScheduledDate" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "CompletedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_HealthEventOccurrences" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_HealthEventOccurrences_HealthEvents_HealthEventId" FOREIGN KEY ("HealthEventId") REFERENCES "HealthEvents" ("Id") ON DELETE CASCADE
);

UPDATE "EventTypeDefinitions" SET "Name" = 'MedicaciÃ³n'
WHERE "Id" = '10000000-0000-0000-0000-000000000002';

UPDATE "EventTypeDefinitions" SET "Name" = 'VacunaciÃ³n'
WHERE "Id" = '10000000-0000-0000-0000-000000000003';

CREATE INDEX "IX_HealthEventOccurrences_HealthEventId" ON "HealthEventOccurrences" ("HealthEventId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260930093256_HealthEventOccurrences', '10.0.12');

COMMIT;

START TRANSACTION;
UPDATE "EventTypeDefinitions" SET "Name" = 'Medicación'
WHERE "Id" = '10000000-0000-0000-0000-000000000002';

UPDATE "EventTypeDefinitions" SET "Name" = 'Vacunación'
WHERE "Id" = '10000000-0000-0000-0000-000000000003';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260930124156_FixEncodingIssues', '10.0.12');

COMMIT;

