# Hackathon Dev Log

## 1. Сводка состояния проекта
- Текущая рабочая сцена: `Assets/DimaD2/Scenes/Prototype_Level_01.unity`.
- Прототип представляет собой мобильную portrait-игру в духе Hole.io на базе простых кастомных скриптов.
- Основной игровой цикл уже собран: стартовый экран, запуск сессии, движение, поглощение, рост, цели, таймер, победа, поражение, пауза, перезапуск.
- Основная логика проекта изолирована в `Assets/DimaD2/`.
- TopDown Engine используется как инфраструктура проекта, без намеренных правок его core-логики.

## 2. Что реализовано
- Создана структура проекта под прототип внутри `Assets/DimaD2/`.
- Собрана и настроена сцена `Prototype_Level_01`.
- Реализовано управление дырой:
  - клавиатура для editor/debug;
  - мобильный dynamic joystick для touch-ввода.
- Реализована камера top-down follow.
- Реализована система поглощаемых объектов:
  - объекты имеют `itemSize` и `itemType`;
  - есть проверка `objectSize <= holeSize`;
  - валидные объекты поглощаются;
  - слишком большие объекты блокируют поглощение.
- Реализована sink-анимация поглощения вместо мгновенного исчезновения.
- Реализована система дискретного роста дыры через `HoleSizeSystem`:
  - прогресс от поглощения;
  - порог роста;
  - рост collider и визуального размера;
  - feedback на absorb / blocked / level up.
- Реализована система целей через `ObjectiveSystem`.
- Реализован `GameStateSystem` с mutually exclusive состояниями win/lose.
- Реализован `TimerSystem` с остановкой на победе/поражении.
- Реализован HUD с таймером и прогрессом целей.
- Реализованы overlay-экраны:
  - Start Panel;
  - Pause Menu;
  - Victory Screen;
  - Lose Screen.
- Реализован flow кнопок:
  - Start;
  - Pause;
  - Resume;
  - Restart / Retry;
  - Exit to Main Menu;
  - Next Level (временно перезапускает ту же сцену).
- Подключен простой absorb/suction audio clip.
- Добавлены generated UI sprites для кнопок, панели и джойстика.
- Настроен Android build launch напрямую в `Prototype_Level_01`.
- Настроены portrait-only orientation и черный splash background.

## 3. Основные этапы итераций
- Подготовлена базовая структура `Assets/DimaD2` и стартовая сцена-прототип.
- Настроены плоскость уровня, камера, placeholder hole и тестовые объекты.
- Добавлено управление движением и слежение камеры.
- Добавлено поглощение объектов с проверкой размера.
- Добавлен рост дыры по порогам.
- Добавлены цели, таймер, win/lose flow и перезапуск сцены.
- Добавлены стартовый экран и пауза.
- Добавлены HUD и overlay-экраны победы/поражения.
- Добавлены mobile-oriented UI style pass и generated sprites.
- Добавлен dynamic joystick и touch input path.
- Исправлены build settings для Android-запуска нужной сцены.
- Исправлены portrait/mobile player settings.
- Исправлялись проблемы сериализованного UI layout в сцене (масштаб корневых canvas, hierarchy panel-объектов, центрирование Victory panel).

## 4. Текущий игровой flow
- Игра открывается сразу в `Prototype_Level_01`.
- Показывается стартовый экран с названием уровня и кратким описанием цели.
- После `Start`:
  - активируется геймплей;
  - включается движение;
  - запускается таймер.
- Игрок двигает дыру и собирает подходящие по размеру объекты.
- Валидные объекты:
  - уходят в sink-анимацию;
  - увеличивают прогресс роста;
  - обновляют цели;
  - проигрывают звук поглощения.
- При выполнении всех целей:
  - останавливается геймплей;
  - показывается Victory screen;
  - кнопка `Next Level` пока просто перезагружает ту же сцену.
- При окончании времени:
  - показывается Lose screen;
  - `Retry` перезагружает сцену.
- Во время активной игры доступно меню паузы.

## 5. Mobile / Android notes
- Build Settings упрощены: в build включена только `Assets/DimaD2/Scenes/Prototype_Level_01.unity`.
- Android orientation переведена в portrait-only.
- Landscape autorotation отключен.
- Splash background переведен в черный.
- UI canvases используют `CanvasScaler` с reference resolution `1080x1920`.
- Safe area fitter используется для start/game HUD.
- Для mobile movement используется floating joystick, который появляется по касанию игровой области.

## 6. UI / input notes
- Все основные UI-элементы находятся в Canvas-based layout.
- Используется generated visual theme для:
  - панелей;
  - primary / secondary / danger buttons;
  - circular pause button;
  - joystick base / knob.
- Клавиатура сохранена как fallback для editor/testing.
- Touch path отделен от UI по `EventSystem.current.IsPointerOverGameObject(...)`.
- Пауза должна блокировать движение и таймер.
- Victory / Lose overlays должны перекрывать игровой экран сверху.

## 7. Известные проблемы / риски
- Главный риск проекта сейчас связан не с core gameplay, а с сериализованным UI layout в сцене:
  - часть правок делалась напрямую по `.unity` YAML;
  - сцену обязательно нужно открыть в Unity и дать ей пересериализоваться.
- Несколько раз обнаруживались некорректные значения `RectTransform.localScale` у корневых Canvas (`0,0,0`) и ошибочная hierarchy у overlay-панелей.
- Victory flow уже исправлялся: ранее победа выглядела как "зависание" из-за отсутствия видимой панели при выключенном gameplay.
- Dynamic joystick path и smoothing правились без live device profiling, поэтому финальное ощущение на Android надо еще руками подтвердить на устройстве.
- В worktree есть сторонние/нецелевые изменения вне `Assets/DimaD2` и `ProjectSettings`, их нельзя бездумно включать в финальный commit.

## 8. Рекомендуемые следующие шаги для разработчиков
- Открыть проект в Unity и проверить Console на import / serialization warnings.
- Прогнать полный playtest в Editor:
  - Start;
  - movement;
  - absorb;
  - growth;
  - objectives;
  - pause/resume;
  - win;
  - lose;
  - retry;
  - next level.
- Прогнать Android build на реальном устройстве и подтвердить:
  - portrait orientation;
  - black splash;
  - корректный safe area;
  - smooth joystick control;
  - видимость Victory/Lose overlays.
- После подтверждения на устройстве можно:
  - почистить и стабилизировать scene hierarchy;
  - убрать лишние debug logs;
  - переименовать `VictoryRetryButton` в более корректное имя (`VictoryNextLevelButton`) без изменения поведения;
  - при необходимости вынести UI layout в более аккуратную prefab-структуру.

## 9. Основные файлы / системы, которые трогались
- Сцена:
  - `Assets/DimaD2/Scenes/Prototype_Level_01.unity`
- Gameplay:
  - `Assets/DimaD2/Scripts/HoleController.cs`
  - `Assets/DimaD2/Scripts/AbsorbableItem.cs`
  - `Assets/DimaD2/Scripts/HoleSizeSystem.cs`
  - `Assets/DimaD2/Scripts/ObjectiveSystem.cs`
  - `Assets/DimaD2/Scripts/GameStateSystem.cs`
  - `Assets/DimaD2/Scripts/TimerSystem.cs`
  - `Assets/DimaD2/Scripts/LevelConfig.cs`
- Camera / input:
  - `Assets/DimaD2/Scripts/TopDownCameraFollow.cs`
  - `Assets/DimaD2/Scripts/TouchJoystickUI.cs`
- UI / flow:
  - `Assets/DimaD2/Scripts/StartPanelUI.cs`
  - `Assets/DimaD2/Scripts/PauseMenuUI.cs`
  - `Assets/DimaD2/Scripts/VictoryUI.cs`
  - `Assets/DimaD2/Scripts/GameHUD.cs`
  - `Assets/DimaD2/Scripts/RestartLevelHandler.cs`
  - `Assets/DimaD2/Scripts/SafeAreaFitter.cs`
  - `Assets/DimaD2/Scripts/MobileUIStyleBootstrap.cs`
- Generated resources:
  - `Assets/DimaD2/Resources/UIGenerated/*`
  - `Assets/DimaD2/Resources/Audio/AbsorbSuction.mp3`
- Build / project settings:
  - `ProjectSettings/EditorBuildSettings.asset`
  - `ProjectSettings/ProjectSettings.asset`
