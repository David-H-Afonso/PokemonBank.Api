/\*\*

- BEAST VAULT API - TypeScript Interfaces
-
- Este archivo contiene TODAS las interfaces TypeScript para el fronte// Filtros de generación
  /** Filtrar por generación donde la especie fue introducida (ej: Rowlet = 7) \*/
  originGeneration?: number;
  /** Filtrar por generación donde fue capturado (ej: Rowlet en SV = 9) _/
  capturedGeneration?: number; que corresponden exactamente a los endpoints y DTOs de la API de Beast /\*\* Clave para identificar el sprite (especie+forma+shiny) _/
  spriteKey: string;
  /** Generación donde la especie fue introducida por primera vez (campo calculado) \*/
  originGeneration: number;
  /** Generación donde este Pokémon específico fue capturado/obtenido (campo calculado) \*/
  capturedGeneration: number;
  }lt.
-
- Generado automáticamente el: 17 de Agosto, 2025
-
- IMPORTANTE: Este archivo debe actualizarse cada vez que cambien los endpoints o DTOs
  \*/

// ===================================
// TIPOS BÁSICOS Y ENUMS
// ===================================

export type ImportStatus = 'imported' | 'duplicate' | 'error';

export type FileFormat = 'pk1' | 'pk2' | 'pk3' | 'pk4' | 'pk5' | 'pk6' | 'pk7' | 'pk8' | 'pk9' |
'pb7' | 'pb8' | 'ek1' | 'ek2' | 'ek3' | 'ek4' | 'ek5' | 'ek6' | 'ek7' | 'ek8' | 'ek9' | 'ekx';

export enum TypeFilterMode {
HasAnyType = 0,
HasAllTypes = 1,
HasOnlyTypes = 2,
PrimaryTypeOnly = 3,
ExactTypeOrder = 4,
BothTypesAnyOrder = 5
}

export enum PokemonSortField {
Id = 0,
PokedexNumber = 1,
SpeciesName = 2,
Nickname = 3,
Level = 4,
OriginGeneration = 5,
CapturedGeneration = 6,
Pokeball = 7,
Gender = 8,
IsShiny = 9,
Form = 10,
CreatedAt = 11,
Favorite = 12
}

export enum SortDirection {
Ascending = 0,
Descending = 1
}

export enum Gender {
Unknown = 0,
Male = 1,
Female = 2
}

// ===================================
// INTERFACES DE RESULTADO
// ===================================

export interface PagedResult<T> {
items: T[];
total: number;
}

export interface ImportResultDto {
/** Nombre del archivo subido \*/
fileName: string;
/** Estado del import: "imported", "duplicate", o "error" _/
status: ImportStatus;
/\*\* ID del Pokémon creado (solo si status es "imported") _/
pokemonId?: number;
/\*_ Mensaje de error (solo si status es "error") _/
message?: string;
}

// ===================================
// INTERFACES DE CONSULTA (QUERY)
// ===================================

export interface PokemonQuery {
/** Búsqueda de texto en nickname o nombre del entrenador original \*/
search?: string;
/** Filtrar por ID de especie (ej: 1 = Bulbasaur) _/
speciesId?: number;
/\*\* Filtrar por ID de forma (ej: 0 = Normal, 1 = Alolan, 2 = Galarian) _/
form?: number;
/** Filtrar por Pokémon shiny \*/
isShiny?: boolean;
/** Filtrar por ID de Pokébola _/
ballId?: number;
/\*\* Filtrar por juego de origen _/
originGame?: number;
/** Filtrar por tipo Tera (Gen 9) \*/
teraType?: number;
/** Número de elementos a saltar (paginación) _/
skip?: number;
/\*\* Número de elementos a devolver (máximo recomendado: 100) _/
take?: number;
}

export interface AdvancedPokemonQuery {
// Filtros básicos
/** Búsqueda de texto en nickname, nombre OT y notas \*/
search?: string;
/** Filtrar por número específico de la Pokédex (Species ID) _/
pokedexNumber?: number;
/\*\* Filtrar por nombre de especie (coincidencia parcial) _/
speciesName?: string;
/** Filtrar por nickname (coincidencia parcial) \*/
nickname?: string;
/** Filtrar por estado shiny _/
isShiny?: boolean;
/\*\* Filtrar por ID de forma _/
form?: number;
/\*_ Filtrar por género (0 = indefinido, 1 = macho, 2 = hembra) _/
gender?: number;

// Filtros de generación
/** Filtrar por generación de origen (juego donde se originó el Pokémon) \*/
originGeneration?: number;
/** Filtrar por generación capturada (generación donde se introdujo la especie) \*/
capturedGeneration?: number;

// Filtros de equipamiento
/** Filtrar por ID de Pokébola \*/
pokeballId?: number;
/** Filtrar por ID de objeto equipado \*/
heldItemId?: number;

// Filtros de tipo
/** ID del tipo primario para filtrado de tipos \*/
primaryType?: number;
/** ID del tipo secundario para filtrado de tipos _/
secondaryType?: number;
/\*\* Modo de filtro de tipos (cómo aplicar los filtros de tipo) _/
typeFilterMode?: TypeFilterMode;
/\*_ Si enforcar el orden exacto de tipos para filtrado de doble tipo _/
enforceTypeOrder?: boolean;

// Filtros de nivel y estadísticas
/** Filtro de nivel mínimo \*/
minLevel?: number;
/** Filtro de nivel máximo \*/
maxLevel?: number;

// Ordenamiento
/** Campo de ordenamiento primario \*/
sortBy?: PokemonSortField;
/** Dirección de ordenamiento primario _/
sortDirection?: SortDirection;
/\*\* Campo de ordenamiento secundario (opcional) _/
thenSortBy?: PokemonSortField;
/\*_ Dirección de ordenamiento secundario _/
thenSortDirection?: SortDirection;

// Paginación
/** Número de elementos a saltar (para paginación) \*/
skip?: number;
/** Número de elementos a tomar (máximo recomendado: 100) \*/
take?: number;

// Soporte legacy
/** @deprecated Usar pokedexNumber en su lugar \*/
speciesId?: number;
/** @deprecated Usar pokeballId en su lugar _/
ballId?: number;
/\*\* Filtro legacy de juego de origen _/
originGame?: number;
/\*_ Filtro legacy de tipo Tera _/
teraType?: number;
}

export interface UpdatePokemonDto {
/** Marcar o desmarcar como favorito (null = sin cambio) \*/
favorite?: boolean;
/** Notas personales sobre el Pokémon (null = sin cambio, string.Empty = limpiar) \*/
notes?: string;
}

// ===================================
// INTERFACES DE RESPUESTA DE DATOS
// ===================================

export interface PokemonListItemDto {
/** ID único del Pokémon en la base de datos \*/
id: number;
/** ID de especie (ej: 1 = Bulbasaur, 25 = Pikachu) _/
speciesId: number;
/\*\* ID de forma (ej: 0 = Meowth Normal, 1 = Meowth de Alola, 2 = Meowth de Galar) _/
form: number;
/** Nickname del Pokémon (null si usa el nombre de la especie) \*/
nickname?: string;
/** Nivel del Pokémon (1-100) _/
level: number;
/\*\* Si es shiny _/
isShiny: boolean;
/** ID de la Pokébola en la que fue capturado \*/
ballId: number;
/** Tipo Tera (Gen 9), null si no aplica _/
teraType?: number;
/\*\* Clave para identificar el sprite (especie+forma+shiny) _/
spriteKey: string;
/** Generación de origen del Pokémon (calculado del juego de origen) \*/
originGeneration: number;
/** Generación en la que fue capturado (calculado del formato de archivo) \*/
capturedGeneration: number;
}

export interface StatsDto {
// IVs (Individual Values)
ivHp: number;
ivAtk: number;
ivDef: number;
ivSpa: number;
ivSpd: number;
ivSpe: number;

// EVs (Effort Values)
evHp: number;
evAtk: number;
evDef: number;
evSpa: number;
evSpd: number;
evSpe: number;

// Hyper Training
hyperTrainedHp: boolean;
hyperTrainedAtk: boolean;
hyperTrainedDef: boolean;
hyperTrainedSpa: boolean;
hyperTrainedSpd: boolean;
hyperTrainedSpe: boolean;

// Estadísticas calculadas actuales
statHp: number;
statAtk: number;
statDef: number;
statSpa: number;
statSpd: number;
statSpe: number;
statHpCurrent: number;
}

export interface MoveDto {
slot: number;
moveId: number;
ppUps: number;
currentPp: number;
}

export interface RelearnMoveDto {
slot: number;
moveId: number;
}

export interface PokemonDetailDto {
// Información básica
id: number;
speciesId: number;
form: number;
nickname?: string;
otName: string;
tid: number;
sid: number;
level: number;
isShiny: boolean;
nature: number;
abilityId: number;
ballId: number;
teraType?: number;
heldItemId: number;
originGame: number;
language: string;
metDate?: string; // ISO date string
metLocation?: string;
spriteKey: string;
favorite: boolean;
notes?: string;
gender: number;
otGender: number;
otLanguage: string;

// Campos mejorados de PK9
encryptionConstant: number;
personalityId: number;
experience: number;
currentFriendship: number;
formArgument: number;
isEgg: boolean;
fatefulEncounter: boolean;
eggLocation: number;
eggMetDate?: string; // ISO date string

// Propiedades físicas
heightScalar: number;
weightScalar: number;
scale: number;

// Pokérus
pokerusState: number;
pokerusDays: number;
pokerusStrain: number;

// Estadísticas de concurso
contestCool: number;
contestBeauty: number;
contestCute: number;
contestSmart: number;
contestTough: number;
contestSheen: number;

// Información del manejador
currentHandler: number;
handlingTrainerName: string;
handlingTrainerGender: number;
handlingTrainerLanguage: number;
handlingTrainerFriendship: number;

// Sistema de memorias
originalTrainerMemory: number;
originalTrainerMemoryIntensity: number;
originalTrainerMemoryFeeling: number;
originalTrainerMemoryVariable: number;
handlingTrainerMemory: number;
handlingTrainerMemoryIntensity: number;
handlingTrainerMemoryFeeling: number;
handlingTrainerMemoryVariable: number;

// Datos relacionados
stats?: StatsDto;
moves: MoveDto[];
relearnMoves: RelearnMoveDto[];
}

// ===================================
// INTERFACES DE METADATA
// ===================================

export interface TypeInfo {
id: number;
name: string;
}

export interface GenerationInfo {
id: number;
name: string;
}

export interface GenderInfo {
id: number;
name: string;
}

export interface SortFieldInfo {
name: string;
value: number;
}

export interface TypeFilterModeInfo {
name: string;
value: number;
}

export interface PokemonMetadata {
types: TypeInfo[];
generations: number[];
genders: GenderInfo[];
sortFields: SortFieldInfo[];
typeFilterModes: TypeFilterModeInfo[];
defaultPageSize: number;
maxPageSize: number;
}

// ===================================
// INTERFACES DE COMPARACIÓN
// ===================================

export interface PokemonComparisonResult {
pokemon1: {
id: number;
species: string;
nickname?: string;
};
pokemon2: {
id: number;
species: string;
nickname?: string;
};
areIdentical: boolean;
differences: PokemonDifference[];
summary: string;
}

export interface PokemonDifference {
field: string;
value1: any;
value2: any;
description: string;
}

// ===================================
// INTERFACES DE SCAN Y MANTENIMIENTO
// ===================================

export interface ScanResult {
success: boolean;
summary: {
totalProcessed: number;
newlyImported: number;
alreadyImported: number;
deleted: number;
errors: number;
};
details: {
newlyImported: ImportResultDto[];
alreadyImported: string[];
deleted: string[];
errors: string[];
};
}

export interface ScanStatus {
directoryExists: boolean;
watchPath: string;
totalPokemonFiles?: number;
filesByExtension?: Record<string, number>;
lastModified?: string; // ISO date string
message?: string;
}

export interface SyncResult {
totalFilesInDatabase: number;
removedFiles: string[];
removedPokemon: string[];
orphanedBackupsFound: number;
orphanedUserFilesFound: number;
mainStorageCleanedUp: number;
syncSummary: string;
success: boolean;
error?: string;
}

export interface FileAnalysisResult {
databaseFiles: {
id: number;
fileName: string;
storedPath: string;
}[];
associatedPokemon: {
id: number;
species: string;
nickname?: string;
}[];
physicalFiles: string[];
backupFiles: string[];
analysis: {
totalDatabaseEntries: number;
totalAssociatedPokemon: number;
totalPhysicalFiles: number;
totalBackupFiles: number;
isConsistent: boolean;
issues: string[];
};
}

// ===================================
// INTERFACES EXTENDIDAS DE RESPUESTA API
// ===================================

export interface AdvancedPokemonListResponse {
items: PokemonListItemDto[];
total: number;
stats: {
queryComplexity: number;
executionTimeMs: number;
filterCount: number;
sortFields: string[];
};
}

export interface HealthCheckResponse {
status: 'ok';
}

export interface WipeDatabaseResponse {
message: string;
deletedBackups: number;
}

export interface DeletePokemonResponse {
deleted: boolean;
fileDeleted: boolean;
backupDeleted?: boolean;
backupPreserved?: boolean;
fileName?: string;
}

// ===================================
// INTERFACES DE ARCHIVO Y EXPORT
// ===================================

export interface FileEntity {
id: number;
sha256: string;
fileName: string;
originalFileName?: string;
format: FileFormat;
size: number;
storedPath: string;
importedAt: string; // ISO date string
rawBlob?: number[]; // byte array
}

// ===================================
// INTERFACES DE SERVICIOS EXTERNOS
// ===================================

export interface GameInfo {
gameId: number;
name: string;
generation: number;
}

export interface SpeciesTypeInfo {
speciesId: number;
primaryType: number;
secondaryType?: number;
}

// ===================================
// TIPOS DE ENDPOINT
// ===================================

export type EndpointResponse<T> = {
data: T;
status: number;
headers: Record<string, string>;
};

export type ApiError = {
message: string;
status: number;
detail?: string;
type?: string;
traceId?: string;
};

// ===================================
// CONSTANTES ÚTILES
// ===================================

export const API_CONSTANTS = {
DEFAULT_PAGE_SIZE: 50,
MAX_PAGE_SIZE: 500,
MAX_LEVEL: 100,
MIN_LEVEL: 1,
SUPPORTED_FILE_EXTENSIONS: [
'.pk1', '.pk2', '.pk3', '.pk4', '.pk5', '.pk6', '.pk7', '.pk8', '.pk9',
'.pb7', '.pb8',
'.ek1', '.ek2', '.ek3', '.ek4', '.ek5', '.ek6', '.ek7', '.ek8', '.ek9',
'.ekx'
] as const,
POKEMON_GENDERS: {
UNKNOWN: 0,
MALE: 1,
FEMALE: 2
} as const
} as const;

// ===================================
// FUNCIONES DE UTILIDAD DE TIPOS
// ===================================

export type PokemonEndpoints = {
// GET endpoints
'/pokemon': {
query: AdvancedPokemonQuery;
response: AdvancedPokemonListResponse;
};
'/pokemon/metadata': {
response: PokemonMetadata;
};
'/pokemon/{id}': {
params: { id: number };
response: PokemonDetailDto;
};
'/pokemon/{id}/showdown': {
params: { id: number };
response: string; // text/plain
};
'/pokemon/compare/{id1}/{id2}': {
params: { id1: number; id2: number };
response: PokemonComparisonResult;
};

// PATCH endpoints
'/pokemon/{id}': {
params: { id: number };
body: UpdatePokemonDto;
response: void; // 204 No Content
};

// DELETE endpoints
'/pokemon/{id}/database': {
params: { id: number };
response: DeletePokemonResponse;
};
'/pokemon/{id}/backup': {
params: { id: number };
response: DeletePokemonResponse;
};
};

export type ImportEndpoints = {
'/import': {
body: FormData; // multipart/form-data with files
response: ImportResultDto[];
};
};

export type FileEndpoints = {
'/files/{id}': {
params: { id: number };
response: Blob; // application/octet-stream
};
'/export/{pokemonId}': {
params: { pokemonId: number };
response: Blob; // application/octet-stream
};
'/export/database/{pokemonId}': {
params: { pokemonId: number };
response: Blob; // application/octet-stream
};
};

export type ScanEndpoints = {
'/scan/directory': {
method: 'POST';
response: ScanResult;
};
'/scan/status': {
response: ScanStatus;
};
};

export type MaintenanceEndpoints = {
'/maintenance/sync': {
method: 'POST';
response: SyncResult;
};
'/maintenance/analyze/{pokemonId}': {
params: { pokemonId: number };
response: FileAnalysisResult;
};
};

export type AdminEndpoints = {
'/admin/wipe-database': {
method: 'POST';
response: WipeDatabaseResponse;
};
};

export type HealthEndpoints = {
'/health': {
response: HealthCheckResponse;
};
};

// Tipo unión de todos los endpoints
export type AllEndpoints = PokemonEndpoints &
ImportEndpoints &
FileEndpoints &
ScanEndpoints &
MaintenanceEndpoints &
AdminEndpoints &
HealthEndpoints;

// ===================================
// COMENTARIOS FINALES
// ===================================

/\*\*

- NOTAS IMPORTANTES PARA EL FRONTEND:
-
- 1.  PAGINACIÓN: Usar skip/take para paginación. El máximo recomendado es take=100.
-
- 2.  FECHAS: Todas las fechas se devuelven como strings ISO (ejemplo: "2025-08-17T15:30:00Z").
- Usar new Date(dateString) para convertir a objetos Date de JavaScript.
-
- 3.  ARCHIVOS: Los endpoints de archivos devuelven Blobs. Usar URL.createObjectURL()
- para crear URLs de descarga.
-
- 4.  FORMDATA: El endpoint de import requiere FormData con archivos.
- Ejemplo: const formData = new FormData(); formData.append('files', file);
-
- 5.  FILTROS AVANZADOS: Usar AdvancedPokemonQuery para consultas complejas con
- filtrado por tipos, generaciones, ordenamiento múltiple, etc.
-
- 6.  TIPOS OPCIONALES: Los campos marcados con ? son opcionales y pueden ser undefined.
-
- 7.  ENUMS: Los enums numéricos deben usarse con sus valores numéricos, no los nombres.
-
- 8.  ERRORES: Todos los endpoints pueden devolver errores HTTP estándar (400, 404, 500).
- Manejar estos errores apropiadamente en el frontend.
-
- 9.  CORS: Asegúrate de que el frontend esté configurado para hacer peticiones al puerto
- correcto de la API (generalmente https://localhost:7xxx o http://localhost:5xxx).
-
- 10. TAGS: Los endpoints están organizados por tags (Pokemon, Import, Files, etc.)
-     para mejor organización en Swagger/OpenAPI.
  \*/
