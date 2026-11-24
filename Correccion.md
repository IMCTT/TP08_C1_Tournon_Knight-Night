# Corrección, Trabajo Práctico N°08 (Platformer 2D)

- **Alumno/a:** Tournon
- **Entrega:** `TP08_C1_Tournon_Knight-Night`
- **Materia:** Programación con Motores de Videojuegos I
- **Nota TP08:** 6

> Todas las rutas son relativas a `Knight Night/`, la carpeta del proyecto de Unity dentro de tu entrega. Los números de línea son los de tus archivos con mis comentarios de corrección ya insertados.

> Hiciste 4 commits después del cierre (2026-11-09 18:00). El commit al cierre es `ac3e1a8` y el último es `5e3551f` (2026-11-10 21:27), con 471 archivos cambiados (5 scripts, incluido el README). Corregí la última versión del repo; no descuenta puntos.

## 1. Checklist de requisitos

### General

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| G1 | Repositorio `TP08_C1_Apellido_Titulo` | ✓ | Carpeta `TP08_C1_Tournon_Knight-Night`, y `.git/config` apunta a `github.com/IMCTT/TP08_C1_Tournon_Knight-Night` |
| G2 | Proyecto de Unity con el mismo nombre | ✗ | El proyecto se llama `Knight Night` (carpeta `Knight Night/`), no `TP08_C1_Tournon_Knight-Night` |
| G3 | Al menos 2 escenas: 1-MainMenu, 2-Gameplay | Parcial | Tenés 2 escenas, pero se llaman `Main Menu.unity` y `Gameplay.unity`, no `1-MainMenu` y `2-Gameplay` como pide la consigna (`ProjectSettings/EditorBuildSettings.asset`) |
| V1 | Publicado en Itch.io y presentable | ✓ | Está publicado en itch.io (revisado el 2026-11-16) |
| V2 | Itch.io jugable en web y descargable para Windows | Parcial | Carga la build web, pero no hay descarga para Windows |

### Jugador

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| J1 | Se mueve a derecha e izquierda | ✓ | `Assets/scripts/Player/Playermovement.cs:155-164` y `:129` |
| J2 | Salta | ✓ | `Assets/scripts/Player/Playermovement.cs:142-149` |
| J3 | Daña enemigos | ✓ | `Assets/scripts/Player/Playermovement.cs:187-205` llama a `EnemyHealth.TakeDamage` |
| J4 | Barra de vida | ✓ | `Assets/scripts/Player/Playermovement.cs:248-251` (`playerLifebar.fillAmount`) |
| J5 | Puede curarse | ✓ | `Assets/scripts/Player/Playermovement.cs:237-246`, prefab `Assets/Prefabs/Meat.prefab` con `PickupHealConfig.asset` |
| J6 | Todas sus acciones animadas y con sonido | Parcial | Suenan el salto, el ataque, el daño, la muerte, la curación, las monedas, los power ups y la llave. Pero `Assets/Art/Animations/Player.controller` solo tiene `Idle`, `run` y `attack`: no hay animación de salto, de daño (solo color y parpadeo, `:261-278`) ni de muerte (el sprite se apaga, `:284`) |

### Enemigos

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| E1 | Dañan al jugador | ✓ | `Assets/scripts/Player/Playermovement.cs:298-308` con `EnemyConfigSO.enemyDamage` |
| E2 | Barra de vida | ✓ | `Assets/scripts/Enemy/EnemyHealth.cs:57-60`, Canvas `HealthBar` en `Assets/Prefabs/Lancer.prefab` |
| E3 | Feedback visual al morir (partículas + animación) | ✓ | `Assets/scripts/Enemy/EnemyHealth.cs:76-79` instancia `DeathParticles.prefab`, y `:95` dispara `Die` hacia `EnemyDeath.anim` |
| E4 | Todas sus acciones animadas y con sonido | Parcial | Recibir golpe y morir tienen sonido (`sfxHit`, `sfxDeath`) y feedback. El enemigo no tiene ninguna otra acción: no hay script de patrulla ni de ataque, y el daño al jugador es por contacto, sin animación ni sonido propio del enemigo (`Lancer.controller` solo tiene `EnemyIdle` y `EnemyDeath`) |

### Pickables

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| K1 | Heal | ✓ | `Assets/scripts/Pickeables/Pickup.cs:28-30`, `Assets/Prefabs/Meat.prefab` (2 instancias en `Gameplay.unity`) |
| K2 | Damage | ✓ | `Assets/scripts/Pickeables/Pickup.cs:31-33`, `Assets/scripts/Player/Playermovement.cs:311-315`, `Assets/Prefabs/Extra Damage.prefab` |
| K3 | Invulnerability por x segundos | ✓ | `Assets/scripts/Player/Playermovement.cs:328-349`, `duration: 5` en `PickupInvConfig.asset` |
| K4 | Reward: monedas | ✓ | `Assets/scripts/Player/Playermovement.cs:318-326`, `Assets/Prefabs/Gold.prefab` (29 instancias), guardadas en `SaveData.Coins`. Además tenés la llave (`Key.prefab`) que habilita `Goal.cs` |

### Sonido

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| S1 | AudioMixer con Master, Music, FX y UI | ✓ | `Assets/SFX/MainMixer.mixer`, con los grupos `Master`, `Music`, `FX` y `Ui` |
| S2 | Al menos 3 grupos con sliders en Settings | ✓ | `Assets/scripts/UI/VolumeSettings.cs:17-20`, 4 sliders en `Main Menu.unity` |
| S3 | Toda acción del juego se refleja en sonido | Parcial | Suenan las acciones del jugador, los enemigos, la UI (`sfx_ui_click.wav`), la victoria y la derrota. El primer pickup de invulnerabilidad y el primero de triple salto no suenan (`Assets/scripts/Player/Playermovement.cs:333` y `:354`) |

### Datos

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| D1 | Configuraciones en ScriptableObject | ✓ | `PlayerConfigSO`, `EnemyConfigSO` y `PickupConfigSO` con sus 8 `.asset` en `Assets/scripts/Data/`, leídos en `Playermovement.Awake` (`:103-110`) y `EnemyHealth.Awake` (`:34`). Quedan números sueltos (mirá las Suggestions de `:322` y `:365`) |
| D2 | Datos de partida en PlayerPrefs | ✓ | `Assets/scripts/Data/SaveData.cs` (monedas y `HighScore`), `Assets/scripts/UI/EndScreen.cs:41-47`, volúmenes en `VolumeSettings.cs:25` |

### Extras

| #  | Extra | Estado | Evidencia |
|----|-------|--------|-----------|
| X1 | Doble salto | ✓ | `maxJumps: 2` en `Assets/scripts/Data/PlayerConfig.asset`, lógica en `Playermovement.cs:137-149` |
| X2 | Triple salto por power up | ✓ | `Assets/scripts/Player/Playermovement.cs:351-372`, `Assets/Prefabs/Cloud.prefab` |
| X3 | Tienda con monedas | ✗ | No hay escena ni script de tienda |
| X4 | Integración PlayFab | ✗ | No hay SDK ni código de PlayFab |
| X5 | Multijugador local | ✗ | Hay un solo jugador |

## 2. Cumplimiento de Pautas de Cursada

| ID | Pauta | Estado | Evidencia |
|----|-------|--------|-----------|
| P1 | Unity 6000.3.21f1 | ✓ | `ProjectSettings/ProjectVersion.txt` dice `6000.3.21f1` |
| P2 | Repo público | ✓ | El repositorio es público (revisado el 2026-11-16) |
| P3 | Nombre del repo | ✓ | `TP08_C1_Tournon_Knight-Night` |
| P4 | Código en inglés | ✓ | En los 13 scripts de `Assets/scripts/`, clases, métodos, campos y enums están en inglés. Escenas `Main Menu` y `Gameplay`. La carpeta `Pickeables` tiene un error de ortografía (`Pickables`) |
| P5 | Un repo por TP, sin branches | ✓ | `git branch -a` muestra solo `main`, sin contenido de otros TPs |

## 3. Estructura de carpetas

> Tu proyecto de Unity está dentro de la subcarpeta `Knight Night/` (`Knight Night/ProjectSettings/ProjectVersion.txt`) y no en la raíz del repositorio.


| Carpeta esperada | Encontrada | Observación |
|------------------|------------|-------------|
| `Scenes/` | ✓ | `Main Menu.unity`, `Gameplay.unity` |
| `Scripts/` | ✓ | Como `scripts/` en minúscula, con `Data/`, `Enemy/`, `Pickeables/`, `Player/`, `UI/` |
| `Prefabs/` | ✓ | 8 prefabs |
| `Art/Sprites/` | ✓ | |
| `Art/Animations/` | ✓ | |
| `Art/Fonts/` | ✓ | |
| `Art/Audio/` | ✗ | El audio está en `Assets/SFX/Audio/` |
| `Data/` | ✗ | Los `.asset` de ScriptableObject están en `Assets/scripts/Data/` |

Archivos mal ubicados:

- `Assets/scripts/Data/*.asset` (8 ScriptableObjects) tendrían que estar en `Assets/Data/`.
- `Assets/SFX/Audio/**` y `Assets/SFX/MainMixer.mixer` tendrían que estar en `Assets/Art/Audio/`.
- `Assets/Art/Particle FX/` tiene sprites (`.png`, `.aseprite`) que van en `Assets/Art/Sprites/`.
- `Assets/scripts/Pickeables/Goal.cs` no es un pickup, va en una carpeta de nivel o de juego.

Ruido de repo: `Assets/Resources/PerformanceTestRunInfo.json` y `PerformanceTestRunSettings.json`, `Assets/Settings/Scenes/URP2DSceneTemplate.unity`, `Assets/InputSystem_Actions.inputactions` (no se usa, el input es por `Input.GetKey`) y `.idea/` versionado. Tenés el `.gitignore` de Unity en `Knight Night/.gitignore`; no hay `.gitattributes`.

## 4. Qué le falta

- Renombrar el proyecto a `TP08_C1_Tournon_Knight-Night` y las escenas a `1-MainMenu` y `2-Gameplay`.
- Animaciones de salto, daño y muerte del jugador.
- Darle al enemigo alguna acción propia (patrulla o ataque) con animación y sonido.
- Sacar `PlaySfx(SfxPowerUp)` de los `if` en `ActivateInvulnerability` y `ActivateCloud`.
- Quitar de `Gameplay.unity` el objeto `Builder`, que tiene un componente con un script faltante.
- Ordenar las carpetas: `Assets/Data/` para los SO y `Assets/Art/Audio/` para el audio.

## 5. Hallazgos de código

### Error (2)
- `Assets/scripts/Player/Playermovement.cs:331` `SfxPowerUp` está dentro del `if (invulnerabilityCoroutine != null)`, así que el primer pickup de invulnerabilidad no suena.
- `Assets/scripts/Player/Playermovement.cs:353` Mismo problema en `ActivateCloud` con el triple salto.

### Bug (3)
- `Assets/scripts/Player/Playermovement.cs:196` `hit.GetComponent<EnemyHealth>()` falla si el círculo toca solo el hijo `DamageColl` del Lancer, que está en la capa Enemy.
- `Assets/scripts/Player/Playermovement.cs:296` El daño por contacto usa `OnTriggerEnter2D`: si el jugador sigue dentro del enemigo cuando termina la invulnerabilidad, no vuelve a recibir daño.
- `Assets/scripts/Pickeables/Goal.cs:22` Si entrás varias veces seguidas a la meta, arrancan corrutinas superpuestas y el mensaje se apaga antes de tiempo.

### Warning (9)
- `Assets/scripts/Player/Playermovement.cs:22` `public bool invincible` se puede modificar desde cualquier clase.
- `Assets/scripts/Player/Playermovement.cs:180` Strings mágicos del Animator en cada frame; usá `Animator.StringToHash`.
- `Assets/scripts/Player/Playermovement.cs:304` `enemy` sin null check antes de leer `enemyDamage`.
- `Assets/scripts/Enemy/EnemyHealth.cs:94` String mágico `"Die"`.
- `Assets/scripts/Enemy/EnemyHealth.cs:96` `deathClip.length` y `sfxDeath.length` sin null check.
- `Assets/scripts/Player/FloorDetect.cs:8` `[SerializeField] public LayerMask groundMask`.
- `Assets/scripts/UI/EndScreen.cs:59` Nombre de escena `"Main Menu"` como string mágico.
- `Assets/scripts/UI/MainMenu.cs:11` Nombre de escena `"Gameplay"` como string mágico.
- `Assets/Scenes/Gameplay.unity` El GameObject `Builder` tiene un MonoBehaviour cuyo script no existe en el proyecto. Unity lo muestra como "Missing Script" y tira un warning al cargar la escena. Borrá el componente o el objeto.

### Suggestion (16)
- `Assets/scripts/Player/Playermovement.cs:7` El nombre de la clase no está en PascalCase y la clase tiene demasiadas responsabilidades.
- `Assets/scripts/Player/Playermovement.cs:15` `[Header]` y valores por defecto en campos privados que después pisa el SO.
- `Assets/scripts/Player/Playermovement.cs:49` Campos privados en PascalCase y sin `private` explícito.
- `Assets/scripts/Player/Playermovement.cs:93` `flip` y `shot` sin uso.
- `Assets/scripts/Player/Playermovement.cs:133` Typo en `Movevement`.
- `Assets/scripts/Player/Playermovement.cs:321` Número mágico `10` para el score.
- `Assets/scripts/Player/Playermovement.cs:323` `coinsText` no se inicializa al arrancar.
- `Assets/scripts/Player/Playermovement.cs:364` `maxJumps = 3` hardcodeado.
- `Assets/scripts/Enemy/EnemyHealth.cs:10` La property `enemyDamage` está en camelCase.
- `Assets/scripts/Enemy/EnemyHealth.cs:23` `shrinkDuration` sin uso.
- `Assets/scripts/Player/FloorDetect.cs:1` `using` sin usar.
- `Assets/scripts/Player/FloorDetect.cs:18` `CheckLayerInMask` es una utilidad metida dentro de `FloorDetect`.
- `Assets/scripts/Pickeables/Pickup.cs:10` `PickupType.Cloud` no describe el efecto.
- `Assets/scripts/UI/VolumeSettings.cs:16` Nombres de parámetros del mixer repetidos como strings.
- `Assets/scripts/Data/SaveData.cs:16` `PlayerPrefs.Save()` en cada moneda.
- `Assets/scripts/Data/PlayerConfigSO.cs:7` Campos `public` en el SO.

## 6. Recomendaciones

1. Renombrá las escenas a `1-MainMenu` y `2-Gameplay` (actualizando `MainMenu.cs:12` y `EndScreen.cs:60`) y el proyecto a `TP08_C1_Tournon_Knight-Night`.
2. Sacá `PlaySfx(SfxPowerUp)` afuera de los `if` en `Playermovement.cs:333` y `:354`.
3. Agregá animaciones de salto, daño y muerte al `Player.controller`, y una acción animada y con sonido para el enemigo (patrulla o ataque).
4. Cambiá `hit.GetComponent` por `hit.GetComponentInParent` en `Playermovement.cs:198` y pasá el daño por contacto a `OnTriggerStay2D` con null check.
5. Borrá el componente con el script faltante del objeto `Builder` en `Gameplay.unity`.
6. Reemplazá los strings de Animator y de escenas por `const` o hashes, y encapsulá `invincible` y `groundMask`.
7. Mové los SO a `Assets/Data/` y el audio a `Assets/Art/Audio/`, y borrá el código muerto (`flip`, `shot`, `shrinkDuration`, `using` sin uso).
