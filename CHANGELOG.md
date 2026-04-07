# Changelog
All notable changes to this project will be documented in this file. See [conventional commits](https://www.conventionalcommits.org/) for commit guidelines.

- - -
## [v0.1.0](https://github.com/Urbaser-S-A/WasteContainerManagement.API/compare/v0.0.0..v0.1.0) - 2026-04-07
#### Features
- (**aspire**) add K8s setup - ([1b1e05c](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/1b1e05cce3571bbe8bb765f13099128797341cd8)) - Alvaro
- (**ci**) add GHA release workflow - ([5da5c3a](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/5da5c3aeea64756951b7792a12b692b0c05fbefe)) - Alvaro
- (**helm**) update templates for passwordless auth with postgres - ([18a760f](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/18a760f6351e60c5b5c6d9298a6deb59d5602f58)) - Alvaro
- (**helm**) update helm manifests - ([46a8479](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/46a8479409ee82de00acb81aad0fca3f5b1c53f7)) - Alvaro
- (**helm**) add helm chart - ([ec44b4b](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/ec44b4bb29c29914a521027fc66601df52fba48a)) - Alvaro
- (**ia**) add agents to project - ([9df0141](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/9df014172c64777e89b9f3626c546c489b4e7660)) - Alvaro
- (**mcp**) add initial mcp setup - ([3470526](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/3470526fae60c239580d19ae0fb02510055bc507)) - Alvaro
- (**openchoreo**) add Wolfi buildpacks build manifests - ([1914846](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/1914846b111efc06723fa015dfb54a41b346640f)) - Alvaro
- (**openchoreo**) add Wolfi buildpack - ([b66f259](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/b66f25996cdfb4843c3606ed22107672208cc120)) - Alvaro
- (**openchoreo**) add Buildpacks build manifests - ([2d3f960](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/2d3f9605015eb1df3609906d4f651cb42b3acc32)) - Alvaro
- (**openchoreo**) add Containerfile source build manifests - ([8a3efec](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/8a3efec279e181b6dfa1d5fe43e2898f3c4c18df)) - Alvaro
- (**openchoreo**) add OpenChoreo base components - ([9c17848](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/9c17848817ad26e484303f1ce26f61ea1123df2e)) - Alvaro
- (**persistence,build**) add auto-migration startup and K8s config support - ([e3ef486](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/e3ef4860d5eb9a0daf9192d3345a4cb250195229)) - Alcantara Escoda, Alejandro
- (**postgres**) fix auth for postgres - ([dbf6feb](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/dbf6feb468004c22e1c15fc5e67ff6f5d69ebf25)) - Alvaro
- (**seed**) enrich seed data for comprehensive endpoint coverage - ([ea03181](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/ea031810e4dfda8cf0ac9e88930487f809d00b53)) - Alcantara Escoda, Alejandro
- (**versioning**) add v2 WasteTypes endpoints with activeContainerCount - ([9f65506](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/9f655062df4de51b556ede34d3dda547910ca355)) - Alcantara Escoda, Alejandro
- (**versioning**) align API versioning with Steritec reference pattern - ([0dbf3a9](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/0dbf3a91c6ad473ee1ec7023cd08a80c1237be72)) - Alcantara Escoda, Alejandro
#### Bug Fixes
- (**db**) skip seeding for external databases - ([518a496](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/518a4962f35874b7bd5e244792910f893d7d12b4)) - Alvaro
- (**logging**) add writeToProviders to Serilog for structured OTLP logs - ([488c472](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/488c472b44c62c7cba3b3c5336fad5eafe1cbabc)) - Alcantara Escoda, Alejandro
- (**middleware**) reorder OutputCache before RateLimiter per Steritec pattern - ([360e85e](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/360e85e46d2bd5d05e3ac31d587e08f8506240d9)) - Alcantara Escoda, Alejandro
#### Refactoring
- (**otel**) add check for app insights env var - ([d832072](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/d832072ce1a5da57afe1e37fcd6219f80c383ff5)) - Alvaro

- - -

## [v0.0.0](https://github.com/Urbaser-S-A/WasteContainerManagement.API/compare/105bebe2e1ef39cdd088c7715df3e216c61df7b6..v0.0.0) - 2026-03-10
#### Features
- (**apphost**) configure Aspire with PostgreSQL container, pgAdmin and data volume - ([0e77b91](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/0e77b911ad3538c9ff0d74d03ceaa8533c12aee8)) - Alcantara Escoda, Alejandro
- (**auth**) configure Azure Entra ID and development authentication per environment - ([6e598a1](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/6e598a1ac90b02e18bf6ae98981c2985ac593fc7)) - Alcantara Escoda, Alejandro
- (**auth**) implement DevelopmentAuthenticationHandler for local development - ([4bbb6d8](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/4bbb6d8ead5a758f8f62f7b5b53d1f348900669b)) - Alcantara Escoda, Alejandro
- (**containers**) implement complete CRUD vertical slice for Containers - ([d7bcd3a](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/d7bcd3a32a7402b9e329ae1cffda9e6fa3727a29)) - Alcantara Escoda, Alejandro
- (**defaults**) configure OpenTelemetry, health checks, service discovery and HTTP resilience - ([87a6e66](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/87a6e66c57116cafa1bb629d66ad226ff71b93af)) - Alcantara Escoda, Alejandro
- (**di**) implement ServiceCollectionExtensions with all cross-cutting service registrations - ([4cef6f6](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/4cef6f6bd6764373e95e1f67f2546b33355610b1)) - Alcantara Escoda, Alejandro
- (**domain**) define entities, enums and domain model for waste container management - ([475d548](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/475d548911c1e199d49d6be5d23a9aaebfc3cef0)) - Alcantara Escoda, Alejandro
- (**domain**) implement Result<T>, Error and DomainErrors shared primitives - ([3163157](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/3163157eb323aee85bedc13dd96f58376f4c6b3e)) - Alcantara Escoda, Alejandro
- (**extensions**) implement ConfigurationExtensions for Azure Key Vault with Managed Identity - ([0543df3](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/0543df36bd7cb7d7782632123dc97b27a86b7bb4)) - Alcantara Escoda, Alejandro
- (**extensions**) implement ResultExtensions for Minimal API result-to-HTTP mapping - ([d86561e](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/d86561eaa699e10a1bf32e82a272ee5616727cd5)) - Alcantara Escoda, Alejandro
- (**incidents**) implement complete CRUD vertical slice for Incidents - ([921608e](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/921608e878c9d0fd9d2f1c5f55dc9d7c76f58d1b)) - Alcantara Escoda, Alejandro
- (**interfaces**) define CRUD repository contracts for all domain entities - ([db1db4f](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/db1db4ffcbea2010c907b3d0425bd4c684db5184)) - Alcantara Escoda, Alejandro
- (**middleware**) implement SecurityHeadersMiddleware with OWASP security headers - ([af0bcb1](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/af0bcb1da9d327a4305666df4332fe616dc6efe5)) - Alcantara Escoda, Alejandro
- (**migrations**) create initial SQL migration script for PostgreSQL schema - ([fd4c38c](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/fd4c38ca017285ed8c641c0e3349b62b44448bbb)) - Alcantara Escoda, Alejandro
- (**openapi**) implement OpenAPI transformers, versioning constants and document registration - ([02fd63d](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/02fd63d12cbc475ebface6d7e24f195ae426edb8)) - Alcantara Escoda, Alejandro
- (**persistence**) implement ApplicationDbContext with PostgreSQL and entity configurations - ([5b01081](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/5b0108134ad56bd90b3f7ee06cb1c5125b3216c2)) - Alcantara Escoda, Alejandro
- (**repositories**) implement BaseRepository with centralized error handling for PostgreSQL - ([e4955ef](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/e4955ef55ed2e00c2299a29ee2ddfa03b4841aec)) - Alcantara Escoda, Alejandro
- (**startup**) configure Serilog and complete Program.cs with full middleware pipeline - ([5be70d5](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/5be70d57c6fd63cf4061f6e1ff3ceea18a611135)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement WasteTypesEndpoints with full CRUD Minimal API routes - ([98f55e0](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/98f55e0acd66c3d01c1d06bbe00fad2c13adb243)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement WasteTypeRepository with EF Core LINQ queries - ([e301919](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/e301919de13e727a0423012993f8fe36b9d7fa7a)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement DeleteWasteType command with active containers validation - ([7e8630a](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/7e8630a5e0a641aa17437e6483ca21b3d1bf783b)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement UpdateWasteType command with validation - ([e0fe688](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/e0fe688925da25ff15da998c3461ea7975df92bb)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement CreateWasteType command with validation - ([a3bb4a2](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/a3bb4a2c88f4922c66be74b58db9b6de0f5ced70)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement GetWasteTypeById query - ([033a8e7](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/033a8e76b44b96eed5c68774193d29a7edec1c29)) - Alcantara Escoda, Alejandro
- (**wastetypes**) implement GetWasteTypes query with DTO, validator and handler - ([1af0860](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/1af0860b0c7a05dc1ebfa86988c76ba6af36a766)) - Alcantara Escoda, Alejandro
- (**zones**) implement complete CRUD vertical slice for Zones - ([2f7a0f6](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/2f7a0f60adfc89bc8733fc8ccccbf7c9dda75e51)) - Alcantara Escoda, Alejandro
#### Tests
- (**handlers**) add unit tests for all CQRS handlers with Moq (44 tests) - ([1eed6d4](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/1eed6d43339626106b0f89dc061605bb9d0f8d26)) - Alcantara Escoda, Alejandro
- (**infrastructure**) add unit tests for middleware and ResultExtensions (26 tests) - ([6e64588](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/6e64588a1855e4bb079b6084901fcb8382594724)) - Alcantara Escoda, Alejandro
- (**validators**) add unit tests for all FluentValidation validators (96 tests) - ([ebd8601](https://github.com/Urbaser-S-A/WasteContainerManagement.API/commit/ebd86010402483956a75b79c702ef11caad6cafb)) - Alcantara Escoda, Alejandro

- - -

Changelog generated by [cocogitto](https://github.com/cocogitto/cocogitto).