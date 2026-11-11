# Knight Night

**Platformer 2D de acción nocturno, inspirado en _Hollow Knight_.**
Trabajo Práctico N°08 · Unity 6 · C#

🎮 Jugar en itch.io: https://sueni.itch.io/knight-night
---

## Sobre el juego

Cae la noche sobre el reino y el camino de vuelta al castillo está lleno de enemigos. Guiá al caballero por la superficie y por las cuevas, esquivá pinches, juntá monedas, encontrá **la llave** y llegá a **la meta del final** para ganar.

## Controles

| Acción | Tecla |
|---|---|
| Moverse | `A` / `D` |
| Saltar (doble salto en el aire) | `W` |
| Atacar con la espada | `R` |

## Cómo se juega

- **Ganás** si llegás al final del nivel **con la llave**. Sin la llave, la meta no se abre.
- **Perdés** si te quedás sin vida. Al tocar enemigos o pinches perdés vida, y después de cada golpe tenés un momento de invulnerabilidad (el caballero parpadea).
- Las **monedas** se guardan entre partidas, y el juego recuerda tu **récord de puntaje**.

## Características

### Jugador
- Movimiento lateral, salto y **doble salto**.
- Ataque con espada que daña a los enemigos.
- Barra de vida, curación con pickups y muerte.
- Animaciones de idle, movimiento y ataque; feedback de daño (flash rojo y parpadeo).
- Sonido en todas sus acciones.

### Enemigos
- Patrullan su zona y dañan al jugador al tocarlo (por trigger).
- Barra de vida propia.
- Al morir: animación de muerte, partículas y sonido.

### Pickables
| Pickup | Efecto |
|---|---|
| ❤️ Heal | Recupera vida |
| ⚔️ Damage | Aumenta el daño de la espada |
| 🛡️ Invulnerability | No recibís daño durante unos segundos |
| 🪙 Coin | Monedas (se guardan entre partidas) |
| 🦘 Triple Jump | Power-up: triple salto por tiempo limitado |
| 🔑 Key | Necesaria para terminar el nivel |

### Nivel
- Dos zonas: **superficie** y **cueva**, conectadas por una caída y una subida con plataformas.
- Pinches, plataformas flotantes y una torre opcional para el triple salto.

### Sonido
- **Audio Mixer** con grupos Master, Music, FX y UI.
- Sliders de volumen en **Settings** para los cuatro grupos (se guardan entre sesiones).
- Música para el menú y para el nivel; sonido en cada acción del juego y en los botones.

### Datos
- **ScriptableObjects** con la configuración del jugador, de los enemigos y de cada pickup.
- **PlayerPrefs** para guardar monedas, récord de puntaje y volúmenes.

### Extras
- ✅ Doble salto.
- ✅ Triple salto a través de un power-up.

## Estructura del proyecto

```
Assets/
├─ Scenes/            1-MainMenu, 2-Gameplay
├─ Scripts/
│  ├─ Player          Playermovement, FloorDetect
│  ├─ Enemies         EnemyHealth, EnemyPatrol
│  ├─ Pickups         Pickup
│  ├─ Data            PlayerConfigSO, EnemyConfigSO, PickupConfigSO, SaveData
│  ├─ UI              MainMenu, VolumeSettings, EndScreen
│  └─ Level           Goal, CameraFollow, LevelBuilder
├─ ScriptableObjects/ Configs del jugador, enemigos y pickups
├─ Audio/             Música y efectos
└─ Art/               Sprites, tiles y fondos
```

## Builds

- **Web (WebGL):** jugable en el navegador desde itch.io.
- **Windows:** descargable desde itch.io.

## Créditos

- **Programación y diseño:** Tomás Tournon
- **Personajes:** [Tiny Swords](https://pixelfrog-assets.itch.io/tiny-swords) — Pixel Frog
- **Terreno y trampas:** [Pixel Adventure 1](https://pixelfrog-assets.itch.io/pixel-adventure-1) — Pixel Frog


