using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Object = UnityEngine.Object;
using System.Xml;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;

namespace GameManager
{
    namespace Hub
    {
        /*===============================================
         *=================== RUNTIME ===================
         *===============================================
         *  All current data relating to gameplay at runtime
         *  is held here. This includes behaviour that relates
         *  to the data stored.
         */
        static class Runtime
        {
            private static GameObject MainCamera { get => Resources.Load<GameObject>("Misc/MainCamera"); }
            private static GameObject Console { get => Resources.Load<GameObject>("Misc/[CONSOLE]"); }

            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                GameObject obj = Object.Instantiate(MainCamera);
                Object.DontDestroyOnLoad(obj);
                obj.name = "MainCamera";

                obj = Object.Instantiate(Console);
                Object.DontDestroyOnLoad(obj);
                obj.name = "[CONSOLE]";
            }

           
        }

        /*===============================================
         *================ AudioManager =================
         *===============================================
         *  Sound and music are centralized here. While objects
         *  in some cases will control sound natively, most will
         *  make use of this class.
         */
        static class AudioManager
        {
            private static GameObject ManagerObject;
            private static AudioSource[] MusicSources;
            private static AudioSource[] SoundSources;
            private static AudioSource AmbianceSource;

            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                //Create Objects to load AudioSource components onto
                ManagerObject = new GameObject("[CORE] AudioManager");
                var musicObj = new GameObject("MusicSources");
                var soundObj = new GameObject("SoundSources");
                var ambianceObj = new GameObject("AmbianceSource");

                //Generate the MusicSoruces
                MusicSources = new AudioSource[3];
                for (int i = 0; i < MusicSources.Length; i++)
                {
                    MusicSources[i] = musicObj.AddComponent<AudioSource>();
                }

                //Generate the SoundSources
                SoundSources = new AudioSource[5];
                for (int i = 0; i < SoundSources.Length; i++)
                {
                    SoundSources[i] = soundObj.AddComponent<AudioSource>();
                }

                //Generate AmbianceSource
                AmbianceSource = ambianceObj.AddComponent<AudioSource>();

                //Parent Manager to Music, Sound and Ambiance objects
                musicObj.transform.SetParent(ManagerObject.transform);
                soundObj.transform.SetParent(ManagerObject.transform);
                ambianceObj.transform.SetParent(ManagerObject.transform);

                Object.DontDestroyOnLoad(ManagerObject);
            }

            public static async void PlayMusic(string request, bool loop, bool fade)
            {
                //Get track requested from Resources/Audio
                AudioClip track = Resources.Load<AudioClip>($"Audio/Music/{request}");
                if (!track)
                {
                    Debug.LogError($"Failed to Play!" +
                        $"\nCould not find track '{request}' in Music folder.");
                    return;
                }

                //Stop currently playing music
                foreach (AudioSource audioSource in PlayingSources(MusicSources))
                {
                    if (!fade)
                    {
                        //Just stop, don't want to fade
                        audioSource.Stop();
                        continue;
                    }

                    //Want to stop by fading
                    await FadeSource(audioSource, 0);
                }

                MusicSources[0].loop = loop;
                MusicSources[0].clip = track;
                MusicSources[0].Play();
            }

            public static void PlaySound(AudioClip audio)
            {
                if (!audio) return;

                AudioSource src = FreeSource(SoundSources);
                src = src == null ? SoundSources[0] : src;

                src.clip = audio;
                src.Play();
            }

            public static async void PlaySound(AudioClip audio, float delay)
            {
                await Task.Delay(Mathf.RoundToInt(delay * 1000));
                PlaySound(audio);
            }

            private static AudioSource FreeSource(AudioSource[] sources)
            {
                foreach (AudioSource src in sources)
                {
                    if (!src.isPlaying) return src;
                }

                return null;
            }

            private static AudioSource[] PlayingSources(params AudioSource[] sources)
            {
                if (sources.Length == 0) return null;

                List<AudioSource> playing = new List<AudioSource>();
                foreach (AudioSource audioSource in sources)
                {
                    if (audioSource.isPlaying) playing.Add(audioSource);
                }

                return playing.ToArray();
            }

            private static async Task FadeSource(AudioSource audioSource, float target)
            {
                //Get 1% of current volume
                var incriment = audioSource.volume == 0 ?
                    0.1f : (audioSource.volume / 100);

                while (audioSource.volume != target)
                {
                    //Add incriment to volume.
                    audioSource.volume += incriment;

                    //If volume close enough to target leave loop
                    if (audioSource.volume <= Mathf.Abs(incriment))
                    {
                        audioSource.volume = target;
                        break;
                    }

                    //Wait 100 miliseconds
                    await Task.Delay(100);
                }

            }
        }

        static class Map
        {
            private static GameObject CurrentMap;
            private static Tilemap ElementLayer;
            private static Dictionary<TileBase, ElementalType> tiles = new();

            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                TileBase[] tileBaseArr = Resources.LoadAll<TileBase>("Misc/TileData");
                foreach (TileBase tBase in tileBaseArr)
                {
                    tiles.Add(tBase, (ElementalType)System.Enum.Parse(typeof(ElementalType), tBase.name));
                }

            }

            public static void SetMap(GameObject map)
            {
                CurrentMap = map;
                List<TilemapRenderer> renderers = CurrentMap.GetComponentsInChildren<TilemapRenderer>().ToList();
                foreach (TilemapRenderer tr in renderers)
                {
                    if (tr.sortingLayerName.Equals("Element"))
                    {
                        ElementLayer = tr.transform.GetComponent<Tilemap>();
                        tr.enabled = false;
                        return;
                    }
                }

                Debug.LogError($"Did not find 'Element' layer for {map.name}");
            }

            public static ElementalType GetElementAtLocation(Vector3Int location)
            {
                if (!CurrentMap)
                {
                    Debug.LogError("No map is currently set, so it's not possible to read from tilemap");
                    return ElementalType.True;
                }

                TileBase tile = ElementLayer.GetTile(location);
                if (!tile)
                {
                    Debug.LogError($"Did not find tile @ {location}");
                    return ElementalType.True;
                }

                if (!tiles.TryGetValue(tile, out ElementalType val))
                {
                    Debug.LogError($"Tile '{tile.name}' was not recognized");
                    return ElementalType.True;
                }

                return val;
            }
        }

        static class UI
        {
            public static GUIController GUI { private set; get; }
            public static BattleUI BattleUI { private set; get; }
            public static EventSystem EventSystem { private set; get; }

            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                //GUI
                var obj = Object.Instantiate(Resources.Load<GameObject>("GUI/GUI"));
                obj.name = "GUI";
                GUI = obj.GetComponent<GUIController>();
                Object.DontDestroyOnLoad(obj);

                //BattleUI
                obj = Object.Instantiate(Resources.Load<GameObject>("GUI/BattleUI"));
                obj.name = "BattleUI";
                BattleUI = obj.GetComponent <BattleUI>();
                Object.DontDestroyOnLoad(obj);

                //EventSystem
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (Object.FindObjectOfType<EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    EventSystem = eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<InputSystemUIInputModule>();
                }
            }
        }

        /*===============================================
         *============== WorldAnimations ================
         *===============================================
         *  Animation objects to be spawned into the world
         *  are regulated through here. It is expected these
         *  animations are made to be prefabs that then have
         *  spawn behaviour defined from this class.
         */
        static class WorldAnimations
        {
            private static List<SkillAnimator> SkillAnims = new List<SkillAnimator>();

            public static void PlaySkillAnimation(string request, params BattleSquare[] squares)
            {
                var prefab = Resources.Load<GameObject>("Battle/SkillAnimator");
                if (!prefab || squares.Length == 0) return;

                foreach (BattleSquare sq in squares) {
                    if (sq == null) continue;

                    var obj = Object.Instantiate(prefab, sq.transform);
                    SkillAnimator anim = obj.GetComponent<SkillAnimator>();
                    anim.Play(request, sq.transform);
                    SkillAnims.Add(anim);
                }
            }

            public static bool SkillPlaying() => SkillAnims.Count > 0;

            public static void Remove(object obj)
            {
                if (obj.GetType() == typeof(SkillAnimator))
                {
                    SkillAnims.Remove(obj as SkillAnimator);
                    return;
                }

                Debug.LogError($"Did not recognize {obj} as animation obj.");
            }
        }

        /*===============================================
         *================= NAVIGATION ==================
         *===============================================
         *  Simple class responsible for basic navigation 
         *  requests. Works in tandem with A*.
         */
        static class Navigation
        {

            // Pointer arrow object that shows current destination
            public static GameObject pointObj { get; private set; }

            /**
             * Any navigation instructions we want to happen at start 
             * are called from this function using RuntimeInitializeOnLoadMethod
             * attribute.
             */
            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                InputManager.PlayerInput().MoveRMB.Enable();
                InputManager.PlayerInput().MoveRMB.performed += evt => SetWayPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                InputManager.PlayerInput().Move.performed += evt => MovePlayer(true);
                InputManager.PlayerInput().Move.canceled += evt => MovePlayer(false);

            }

            /**
             * Attempts to send target to Player unit. If desired location is
             * valid a new pointObj is instantiated at destination.
             * 
             * @param Vector2
             */
            public static void SetWayPoint(Vector2 location)
            {
                var player = Units.UnitManager.Player;
                if (!ValidWayPoint()) return;
                if (!player) return;
                if (Vector2.Distance(player.transform.position, location) < 0.5f) return;

                Vector2 newLoc = CenterSquare(location);

                if (pointObj) Object.Destroy(pointObj);
                pointObj = Object.Instantiate(Resources.Load<GameObject>("Misc/MovePoint"));
                pointObj.transform.position = newLoc;
                player.pathFinder.GoTo(pointObj.transform);
            }

            public static Vector2 CenterSquare(Vector2 location) {
                int x = Mathf.FloorToInt(location.x);
                int y = Mathf.FloorToInt(location.y);
                return new Vector2((x + 0.5f), (y + 0.5f));
            }

            private static void MovePlayer(bool active) {
                if (!Units.UnitManager.Player) return;
                Units.UnitManager.Player.movementOverride = active;
            }

      
            //Returns true if given postion (Vector2) in inhabited by an obstacle.
            public static bool ObstacleAtLocation(Vector2 postion)
            {
                //Check a collider is in the area
                Collider2D coll = Physics2D.OverlapArea(new Vector2(postion.x - .3f, postion.y - .3f), new Vector2(postion.x + .3f, postion.y + .3f));
                if (coll == null) return false; // Nothing is there

                // Check collider belongs to object on Obstacle layer
                return coll.gameObject.layer == 3;
            }

            //Returns true if collider exists at given postion (Vector2).
            public static Collider2D ColliderAtLocation(Vector2 position) {
                Vector2 x = position - ((Vector2.left + Vector2.up) / 4);
                Vector2 y = position - ((Vector2.right + Vector2.down) / 4);
                Collider2D coll = Physics2D.OverlapArea(x, y);

                return coll;
            }

            //Returns false if collider exists at given postion (Vector2) and collider is tagged "NotItemSafe".
            public static bool ItemSafeLocation(Vector2 position) { 
                var coll = ColliderAtLocation(position);

                return coll == null || !coll.CompareTag("NotItemSafe");
            }

            /**
             * Checks if the mouse location is over an obstacle. If so, we know
             * the player can't go there and so return false.
             * 
             * @return bool
             */
            private static bool ValidWayPoint()
            {
                // Generate Ray object to mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // If no collider or isTrigger then it's valid
                return hit.collider == null || hit.collider.isTrigger;
            }

            /**
             * Destroys pointObj from scene.
             */
            public static void DestoyWayPoint()
            {
                if (!pointObj) return;

                if(Units.UnitManager.Player) {
                    Units.UnitManager.Player.pathFinder.Clear();
                }
                Object.Destroy(pointObj);
                pointObj = null;
            }

        }
    }
    
    namespace Units
    {
        /*===============================================
         *================= UNIT MANAGER ================
         *===============================================
         *  Class for Unit functionality and
         *  behaviour.
         */
        static class UnitManager
        {
            public static UnitController Player;
            public static UnitController[] Party = new UnitController[0];
            public static List<EnemyUnit> EnemyUnits = new List<EnemyUnit>();

            /**
             * Uses UnitInstance type to spawn unit based on ID
             * at specified location. 
             * 
             * @param unitID
             */
            public static UnitController Instantiate(UnitInstance unitInstance)
            {
                GameObject unitObj = Resources.Load<GameObject>("Units/" + unitInstance.unitID);
                if (!unitObj)
                {
                    Debug.LogError("[UnitManager] ERROR: No unit object found for: " +
                        ("SPUM/SPUM_Units/" + unitInstance.unitID));
                    return null;
                }

                GameObject newUnit = Object.Instantiate(unitObj);
                newUnit.transform.position = unitInstance.spawnLocation;
                newUnit.name = $"{unitObj.name} [U]";

                if (unitInstance.isPlayable) {
                    SetPlayer(newUnit);
                }
                    

                return newUnit.GetComponent<UnitController>();
            }

            /**
            * Assigns external unit as the Player object. The gameObject
            * that is passed as parameter must have UnitController component
            * to work.
            * 
            * @param GameObject
            */
            public static void SetPlayer(GameObject playableObj)
            {
                var controller = playableObj.GetComponent<UnitController>();
                if (!controller)
                {
                    Debug.LogError(playableObj.name + " does not have <UnitController> component, " +
                        "and so can't be assigned as playable.");
                    return;
                }
                if (Player) Player.tag = "Untagged";
                playableObj.tag = "Player";
                Player = controller;

                CameraController.main.target = Player.transform;
            }

            public static UnitController[] FullParty()
            {
                UnitController[] full = new UnitController[Party.Length + 1];
                full[0] = Player;
                for (int i = 0; i < full.Length; i++)
                    full[i] = (i == 0) ? Player : Party[i - 1];

                return full;
            }

            /**
             * Add Unit to Party array and assigns them the nearest
             * ally to follow.
             * 
             * @param UnitController
             */
            public static void AssignPartyMemeber(UnitController unit)
            {
                if (!unit) return;

                unit.pathFinder.GoTo(Party.Length >= 1 ? Party[Party.Length - 1].transform : Player.transform, true);
                unit.pathFinder.nextToTarget = true;
                List<UnitController> party = Party.ToList();
                party.Add(unit);
                Party = party.ToArray();
            }

            /**
             * Remove Unit from Party array. Removed units stop following
             * any previously assigned target.
             * 
             * @param UnitController
             */
            public static void RemovePartyMemeber(UnitController unit)
            {
                if (!unit) return;

                unit.pathFinder.targetLock = false;
                List<UnitController> party = Party.ToList();
                party.Remove(unit);
                Party = party.ToArray();
            }

            public static void SetUnitTargetLock(bool locked, params UnitController[] units)
            {
                foreach (UnitController uc in units)
                    uc.pathFinder.targetLock = locked;
            }
        }

        /*===============================================
         *================ UNIT INSTANCE ================
         *===============================================
         *  Holds basic variables useful for instantiating
         *  units into scenes.
         */
        [System.Serializable]
        public struct UnitInstance
        {
            public string unitID;
            public Vector2 spawnLocation;
            public bool isPlayable;

            /**
             * Shorthand constructor for non-playable unit
             * to spawn at (0, 0)
             * 
             * @param unitID
             */
            public UnitInstance(string unitID)
            {
                this.unitID = unitID;
                spawnLocation = Vector2.zero;
                isPlayable = false;
            }

            /**
            * Constructs this UnitInstance with given
            * parameters.
            * 
            * @params unitID - spawnLocation - isPlayable
            */
            public UnitInstance(string unitID, Vector2 spawnLocation, bool isPlayable)
            {
                this.unitID = unitID;
                this.spawnLocation = spawnLocation;
                this.isPlayable = isPlayable;
            }
        }

    }
    namespace Battle
    {
        /*===============================================
         *=============== BATTLE SYSTEM =================
         *===============================================
         *  
         */
        static class BattleSystem
        {
            public static List<EnemyUnit> Enemies = new List<EnemyUnit>();
            public static List<UnitController> Party = new List<UnitController>();
            public static List<UnitController> BattleUnits = new List<UnitController>();
            public static Stack<UnitController> TurnOrder = new Stack<UnitController>();

            public static BattleState State { get; private set; }
            private static UnitController CurrentUnit;


            public static async void GoBattle(params EnemyUnit[] enemyArr)
            {
                if (enemyArr.Length == 0) return;
                if (Enemies.Count == 0) SetUpBattle();

                //Add Enemy to battle
                foreach (EnemyUnit enemy in enemyArr)
                {
                    enemy.FaceLocation(Units.UnitManager.Player.transform.position);
                    enemy.AnimationTrigger("Suprise");
                    Enemies.Add(enemy);
                }

                await Task.Delay(1000);

                BattleSquare[] enemyPos = BattleGrid.EnemyStartingPositions(enemyArr.Length);
                for (int i = 0; i < enemyPos.Length; i++) {
                    BattleGrid.UnitToSquare(enemyArr[i], enemyPos[i]);
                    Debug.Log($"E:{enemyArr[i]} | P:{enemyPos[i]}");
                }

            }

            public static UnitController GetCurrentUnit() => CurrentUnit;

            private static async void SetUpBattle()
            {
                Units.UnitManager.Player.StopMoving();
                InputManager.PlayerInput().Disable();
                SetUpGrid();

                await Task.Delay(50);

                //Start battle intro
                Hub.AudioManager.PlayMusic("BattleJazz", true, false);
                CameraController.main.cameraLock = true;
                CameraController.main.ZoomCamera(false);

                Party = new List<UnitController>(Units.UnitManager.FullParty());
                Units.UnitManager.SetUnitTargetLock(false, Party.ToArray());

                //Party Starting Positions
                BattleSquare[] partyPos = BattleGrid.PartyStartingPositions();
                for (int i = 0; i < partyPos.Length; i++) {
                    Party[i].pathFinder.nextToTarget = false;
                    BattleGrid.UnitToSquare(Party[i], partyPos[i]);
                }

                await Task.Delay(5000);

                NextTurn();
            }

            //Process for each individual turn.
            public static async void NextTurn()
            {
                State = CurrentBattleState();
                if (State != BattleState.Active)
                {
                    //TODO: BATTLE END! Process win/lose here...
                    return;
                }

                //Get UnitChoices for this turnset.
                if (TurnOrder.Count() == 0) 
                    StackTurnOrder();

                CurrentUnit = TurnOrder.Pop();
                if (CurrentUnit.combatant.isDead)
                {
                    //This unit is dead. Go to next ...
                    NextTurn();
                    return;
                }

                CameraController.main.CenterBounds(BattleUnits);

                CurrentUnit.combatant.MP = CurrentUnit.combatant.MAXMP();
                if (CurrentUnit.IsEnemy())
                {
                    BattleUI.instance.Disable();
                    EnemyAI enemyAI = CurrentUnit.GetComponent<EnemyAI>();
                    if (!enemyAI) {
                        Debug.LogError("Enemy unit contains no AI behaviour!");
                        NextTurn();
                        return;
                    }

                    Debug.Log("EnemyAI is taking over ...");
                    enemyAI.GenerateInitialModuleTable();
                    while (!enemyAI.NullConfidence()) {
                        enemyAI.NextMove();
                        await Task.Delay(3000);
                    }

                    NextTurn();
                }
                else
                {
                    //Player turn. Set everything up.
                    Debug.Log($"Player Turn. [{CurrentUnit.name} : {TurnOrder.Count()}]");
                    BattleEvent.InvokePartyMemberChangeEvent();
                    BattleUI.instance.Enable();
                }

                //Set grid back to normal
                BattleGrid.SetGridState(BattleSquare.BattleSquareState.Closed);
            }

            public static void InvokeInstruction(BattleSquare casterSqaure, BattleSquare[] targetSquares, Instruction instruction)
            {
                float x = 0;
                Instruction.Action action = instruction.action;
                UnitController casterUnit = casterSqaure.GetInhabitedUnits()[0];

                switch (action.evt)
                {
                    case Instruction.ActionEvent.KnockBack: //KNOCKBACK

                        break;
                    case Instruction.ActionEvent.Damage:    //DAMAGE
                        foreach (BattleSquare sq in targetSquares)
                        {
                            Elemental[] elementals = Parse.ParseDamage(action.value, casterSqaure, sq);
                            foreach (UnitController unit in sq.GetInhabitedUnits())
                            {
                                x = unit.combatant.TakeDamage(casterUnit.combatant, elementals);
                                unit.AnimationTrigger("Hit");
                                Debug.Log($"Invoked [{x}] Damage on {unit.name}");
                            }

                            sq.SetActiveElement(Elemental.Combine(sq.activeElement, Parse.ParseActiveElement(action.value)));
                            sq.UpdateHealthBar();
                        }

                        break;
                }

                if (casterUnit.IsEnemy()) return;
                else if (casterUnit.combatant.MP == 0) {
                    NextTurn();
                } 
            }

            private static void StackTurnOrder()
            {
                TurnOrder.Clear();
                BattleUnits.Clear();

                for (int i = 0; i < (Party.Count() + Enemies.Count()); i++)
                {
                    if (i > Party.Count() && i > Enemies.Count()) break;

                    //Attempt to add Party @ i
                    if (i < Party.Count())
                    {
                        if (!Party[i].combatant.isDead)
                            BattleUnits.Add(Party[i]);
                    }

                    //Attempt to add Enemy @ i
                    if (i < Enemies.Count())
                    {
                        if (!Enemies[i].combatant.isDead)
                            BattleUnits.Add(Enemies[i]);
                    }
                }


                BattleUnits = BattleUnits.OrderBy(x => x.combatant.MAXMP()).ToList();

                foreach (UnitController unit in BattleUnits) {
                    TurnOrder.Push(unit);
                    Debug.Log($"Turn: {unit}");
                }
            }

            private static void SetUpGrid()
            {
                var max = 6f;   // Always minimum 8x8 grid
                foreach (EnemyUnit unit in Enemies)
                {
                    var dist = Vector2.Distance(Units.UnitManager.Player.transform.position, unit.transform.position);
                    max = (dist > max) ? dist : max;
                    if (max > 8) break; //Max possible size of grid will be 8
                }

                // Create new grid based on max distance
                BattleGrid.NewGrid(Mathf.RoundToInt((max <= 8) ? max : 8));
            }

            public static EnemyUnit GetClosestEnemy()
            {
                if (Enemies.Count == 0) return null;

                EnemyUnit closest = Enemies[0];
                if (Enemies.Count == 1) return closest;

                float closestDist = Vector2.Distance(Party[0].transform.position, closest.transform.position);
                for (int i = 1; i < Enemies.Count; i++) {
                    float enemyDis = Vector2.Distance(Party[0].transform.position, Enemies[i].transform.position);
                    if (enemyDis > closestDist) continue;

                    closest = Enemies[i];
                    closestDist = enemyDis;
                }

                return closest;
            }

            public static BattleState CurrentBattleState()
            {
                if (Party.Count == 0 || Enemies.Count == 0) return BattleState.InActive;
                if (!BattleGrid.HasGrid()) return BattleState.InActive;

                bool partyActive = false;
                foreach (UnitController unit in Party)
                {
                    if (!unit.combatant.isDead)
                    {
                        partyActive = true;
                        break;
                    }
                }

                if (!partyActive) return BattleState.EnemyWin;

                foreach (UnitController unit in Enemies) {
                    if (!unit.combatant.isDead) return BattleState.Active;
                }

                return BattleState.PartyWin;
            }
        }

        /*===============================================
         *================ BATTLE GRID ==================
         *===============================================
         *  BattleGrid defines and instances a 'chessboard-like'
         *  arena for battles to take place in. When 'NewGrid' is
         *  called, creates multiple 'SquarePrefab' in area around
         *  the player to size based on parameter. 
         */
        static class BattleGrid
        {
            private static BattleSquare[,] GridObjs = new BattleSquare[0, 0];   //All SquarePrefab objects currently instantiated.
            public static List<BattleSquare> InhabitedSquares = new List<BattleSquare>();  //All currently inhabited squares
            private static GameObject Parent;       //Store all SquarePrefab gameObjects into Parent.
            private static GameObject SquarePrefab;     //Base prefab representing a Square in our grid.
            public static BattleSquare Selected = null;     //Current square selected by mouse
            public static bool awaitingSquareSelection;

            /**Called automatically at Application start. */
            [RuntimeInitializeOnLoadMethod]
            private static void OnLoad()
            {
                SquarePrefab = Resources.Load<GameObject>("Battle/Square");
                if (!SquarePrefab)
                    Debug.LogAssertion("Could not find Resources/Battle/Square. " +
                        "Battles will not function correctly!");
            }

            /**
             *  Creates new BattleGrid around the player 
             *  based on parameter size. Any non-inhabitable
             *  locations are not assigned a square and so
             *  are null.
             *  
             *  @param size
             */
            public static void NewGrid(int size)
            {
                var player = Units.UnitManager.Player;    //Get Player
                if (!player) return;    //Can't center grid with no player.

                ClearGrid();    //Destroy any previous grid
                GridObjs = new BattleSquare[size, size];    //Initialize dynamic array to fit size
                InhabitedSquares.Clear();   //Clear Inhabited list

                var playerPos = player.transform.position;  //Get Player position
                Vector2 start = new (playerPos.x - Mathf.Abs(size / 2),   // x
                                                playerPos.y - Mathf.Abs(size / 2)); // y

                for (int y = 0; y < size; y++) {
                    for (int x = 0; x < size; x++) {
                        Vector2 location = new (start.x + x, start.y + y); //Get next location
                        if (ValidSquareLocation(location))
                        {    //Is the location inhabitable?
                            //Create new square at location
                            BattleSquare newSq = InstantitateSquare(location);
                            newSq.name = $"BattleSquare ({x},{y})";
                            GridObjs[x, y] = newSq;
                        }
                    }
                }

                BattleEvent.PartyMemberChangeEvent += ResetGrid;
            }

            /**
             * Returns true if a BattleGrid has been instanciated.
             * 
             * @return bool
             */
            public static bool HasGrid()
            {
                return GridObjs.Length > 0;
            }

            /**
             * Sets player selected BattleSquare to target array
             * in current party member Instruction.
             */
            public static void SquareSelect()
            {
                if (!awaitingSquareSelection || !Selected) return;

                awaitingSquareSelection = false;
                BattleEvent.InvokeSquareCallEvent();
            }

            /**
             * Destroys any current instance of BattleGrid
             * and sets GridObjs[0, 0] 
             */
            public static void ClearGrid()
            {
                //Destroy all BattleSquare objects from scene.
                foreach (BattleSquare sq in GridObjs)
                    if (sq) Object.Destroy(sq.gameObject);

                //Set dynamic array to empty.
                GridObjs = new BattleSquare[0, 0];
                BattleEvent.PartyMemberChangeEvent -= ResetGrid;
            }

            public static BattleSquare[] GetSquares(BattleSquare center, Pattern pattern, params BattleSquare.BattleSquareState[] states)
            {
                List<BattleSquare> squares = new List<BattleSquare>();
                var pmVector = Coordinates(center);

                //For every element in sequence of patterns
                foreach (Vector2 element in pattern.sequence)
                {
                    var previous = center;

                    //Over specified iterations
                    for (int i = 1; i <= pattern.iterations; i++)
                    {
                        //Index of next potential square
                        var next = (pmVector + (element * i));

                        //In bounds?
                        if ((int)next.x < GridObjs.GetLength(0) && (int)next.y < GridObjs.GetLength(1) &&
                        (int)next.x >= 0 && (int)next.y >= 0)
                        {

                            //Square at location?
                            var sq = GridObjs[(int)next.x, (int)next.y];
                            if (!sq) continue;

                            //Square only 1 away from previous square?
                            if (Vector2.Distance(sq.transform.position, previous.transform.position) > 1.1f)
                                break;

                            previous = sq;

                            //Square viable?
                            if (states.Length == 0)
                            {
                                if (sq && pattern.StateMatch(sq))
                                    squares.Add(GridObjs[(int)next.x, (int)next.y]);
                                else break;
                            }
                            else
                            {
                                //Check given param states
                                var unt = sq.GetInhabitedUnits().Length > 0 ? sq.GetInhabitedUnits()[0] : null;
                                foreach (BattleSquare.BattleSquareState s in states)
                                    if (sq.state == s)
                                        squares.Add(GridObjs[(int)next.x, (int)next.y]);
                            }

                        }
                    }
                }

                if (squares.Count == 0)
                {
                    Debug.LogWarning("No squares were found for pattern: " + pattern.ToString());
                    return new BattleSquare[0];
                }

                return squares.ToArray();
            }

            public static BattleSquare ClosestSquare(Vector2 coordinate)
            {
                BattleSquare closestSquare = GetSquareAtCoordinate(coordinate);
                int x = (int)coordinate.x;
                int y = 0;

                //If no square here, keep getting next square until not null
                while (closestSquare == null || closestSquare.ContainsUnits())
                {
                    closestSquare = GetSquareAtCoordinate(new Vector2(x, y));

                    if (y >= GridObjs.GetLength(0))
                    {
                        y = 0;
                        x = coordinate.x == 0 ? x++ : x--;
                    }
                    else
                    {
                        y++;
                    }

                    if (x < 0 || x == GridObjs.GetLength(1)) break;
                }

                return closestSquare;
            }

            public static BattleSquare GetSquareAtCoordinate(Vector2 coordinate)
            {
                try
                {
                    return GridObjs[(int)coordinate.x, (int)coordinate.y];
                }
                catch
                {
                    return null;
                }
            }

            public static BattleSquare GetCurrentSquare() => GetSquareViaUnit(BattleSystem.GetCurrentUnit());

            /**
             * Attempts to get availible squares for
             * current party members movement. All closed
             * squares within a pattern are accepted while
             * accepting inhabited squares if not enemy unit.
             * 
             * return bool
             */
            public static BattleSquare[] GetMoveSquares(UnitController unit)
            {
                List<BattleSquare> squares = new List<BattleSquare>();
                var pmSQ = GetSquareViaUnit(unit);
                var pmVector = Coordinates(pmSQ);
                Pattern pattern = unit.combatant.Movement.pattern;

                
                //For every element in sequence of patterns
                foreach (Vector2 element in pattern.sequence)
                {

                    //Over specified iterations
                    for (int i = 1; i <= pattern.iterations; i++)
                    {
                        //Index of next potential square
                        var next = (pmVector + (element * i));

                        //In bounds?
                        if ((int)next.x < GridObjs.GetLength(0) && (int)next.y < GridObjs.GetLength(1) &&
                        (int)next.x >= 0 && (int)next.y >= 0)
                        {
                            //Square at location?
                            var sq = GridObjs[(int)next.x, (int)next.y];

                            if (sq)
                            {

                                //Is this square inhabited
                                if (sq.ContainsUnits())
                                {
                                    UnitController inhabitant = sq.GetInhabitedUnits()[0];

                                    //Is the mover allied with inhabitant?
                                    if (inhabitant.IsEnemy() == unit.IsEnemy() && !inhabitant.combatant.isDead)
                                    {
                                        //Unit is ally and alive
                                        squares.Add(GridObjs[(int)next.x, (int)next.y]);
                                    }

                                    break;
                                }

                                //Not inhabited square, so we just add.
                                squares.Add(GridObjs[(int)next.x, (int)next.y]);
                            }

                        }

                    }
                }


                foreach (BattleSquare sq in squares)
                {
                    sq.SetState(BattleSquare.BattleSquareState.Open);
                    if (sq.ContainsUnits()) sq.SetColor(Color.red);
                }

                return squares.ToArray();
            }

            public static UnitController[] GetUnitsInPattern(BattleSquare center, Pattern pattern, bool playerUnits = false)
            {
                List<UnitController> units = new List<UnitController>();
                BattleSquare[] squares = GetSquares(center, pattern);

                foreach (BattleSquare sq in squares)
                {
                    UnitController[] contents = sq.GetInhabitedUnits();
                    foreach (UnitController u in contents)
                    {
                        if (playerUnits)
                        {              //Looking for party units
                            if (!u.IsEnemy())           //Unit is not an Enemy
                                units.Add(u);           //Add to units array
                        }
                        else
                        {                          //Looking for enemy units
                            if (u.IsEnemy())            //Unit is an enemy 
                                units.Add(u);           //Add to units array
                        }
                    }
                }

                return units.ToArray();
            }

            /**
             * Gets all squares within 1 distance of given BattleSquare
             * 
             * return BattleSquare
             */
            public static BattleSquare[] Radial(BattleSquare square) {
                List<BattleSquare> squares = new List<BattleSquare>();

                Vector2[] radialV2 = new Vector2[] { 
                    Vector2.down, Vector2.left, Vector2.right, Vector2.up,
                    (Vector2.up + Vector2.right), ((Vector2.down + Vector2.right)),
                    (Vector2.down + Vector2.left), (Vector2.up + Vector2.left)
                };

                Vector2 center = Coordinates(square);
                foreach(Vector2 v2 in radialV2) {
                    BattleSquare sq = GetSquareAtCoordinate(center + v2);
                    if (sq) squares.Add(sq);
                }

                return squares.ToArray();
            }

            public static int Distance(BattleSquare x, BattleSquare y)
            {
                Vector2 vX = Coordinates(x);
                Vector2 vY = Coordinates(y);
                bool diagonal = !(vX.x == vY.x) && !(vX.y == vY.y);

                return Mathf.CeilToInt(Vector2.Distance(vX, vY) - (diagonal ? 1 : 0));
            }

            public static void SetGridState(BattleSquare.BattleSquareState state, params BattleSquare[] squares)
            {
                if (squares.Length > 0)
                {
                    foreach (BattleSquare sq in squares)
                        sq.SetState(state);
                    return;
                }

                for (int x = 0; x < GridObjs.GetLength(0); x++)
                    for (int y = 0; y < GridObjs.GetLength(1); y++)
                        if (GridObjs[x, y]) GridObjs[x, y].SetState(GridObjs[x, y].ContainsUnits() ?
                            BattleSquare.BattleSquareState.Inhabited : state);

            }

            /**
             * Sends unit to specified BattleSquare.
             * 
             * @params UnitController, BattleSquare
             */
            public static void UnitToSquare(UnitController unit, BattleSquare square) {
                var prevSQ = GetSquareViaUnit(unit);
                if (!prevSQ)
                {
                    square.SetState(BattleSquare.BattleSquareState.Inhabited, unit);
                    return;
                }

                if (square.ContainsUnits())
                {
                    prevSQ.SetState(BattleSquare.BattleSquareState.Inhabited, square.GetInhabitedUnits());
                }
                else
                {
                    prevSQ.SetState(BattleSquare.BattleSquareState.Closed);
                }

                square.SetState(BattleSquare.BattleSquareState.Inhabited, unit);
            }

            public static void UnitToSquare(UnitController unit, BattleSquare square, bool charge) {
                int dist = Distance(GetSquareViaUnit(unit), square);
                if (dist > unit.combatant.MP) return;
                if (charge) unit.combatant.MP -= dist;

                UnitToSquare(unit, square);
            } 

            /**
             * Returns the square given unit is inhabiting, null
             * if unit is not on any squares.
             * 
             * return BattleSquare
             */
            public static BattleSquare GetSquareViaUnit(UnitController unit)
            {
                foreach (BattleSquare square in InhabitedSquares)
                    if (square.ContainsUnits(unit)) return square;

                return null;
            }

            public static BattleSquare CurrentUnitSquare() => GetSquareViaUnit(BattleSystem.GetCurrentUnit());

            /**
             * Calculates and returns starting positions based on
             * how many enemies inhabit the BattleGrid.
             * 
             * @param int
             * @return BattleSquare[]
             */
            public static BattleSquare[] EnemyStartingPositions(int enemyCount)
            {
                int size = GridObjs.GetLength(1) - 1;   //Get (column) size of grid

                Debug.LogWarning($"There are {BattleSystem.Enemies.Count} enemies ...");
                bool rightSide = BattleSystem.GetClosestEnemy().transform.position.x > BattleSystem.Party[0].transform.position.x;
                int x = rightSide ? size : 0;     //Set x (column) to side.
                int y = 0;    //Go from bottom (row)

                var squares = new BattleSquare[enemyCount];
                for (int i = 0; i < enemyCount; i++)
                {
                    var sq = GridObjs[x, y];   //Pull next square

                    #region Skip non-inhabitable squares
                    while (sq == null || sq.ContainsUnits())
                    {
                        //Is a valid square there? (null = non-inhabitable)
                        Debug.Log($"Invalid: ({x}, {y}) for [{(i + 1)}] " +
                            $"\nProblem: {(sq == null ? "NULL Square" : "Contains Units")}");
                        if ((y + 1) > size)
                        {  //Are we at top?
                            y = 0;
                            x = rightSide ? (x - 1) : (x + 1);    //...and go no next column
                            Debug.Log($"Snap: {x} R[{rightSide}]");
                            continue;
                        }

                        y++;    //We can go across
                        sq = GridObjs[x, y];    //Check next square
                    }
                    #endregion

                    squares[i] = sq;    //Set available square
                    if ((y + 1) > size)
                    {
                        y = 0;
                        x = rightSide ? x-- : x++;    //...and go no next column
                        continue;
                    }

                    y++;
                }

                //Returns squares found to be used as starting positions for enemies.
                return squares;
            }

            /**
             * Calculates and returns starting positions based
             * on Party size. Positions skew to left-most side.
             * 
             * @return BattleSquare[]
             */
            public static BattleSquare[] PartyStartingPositions()
            {
                int size = GridObjs.GetLength(1);   //Get size of grid
                int partySize = Units.UnitManager.FullParty().Length; //Get size of Party
                int y = ((size + partySize) / 3) + 1;   //Find center of y

                Vector2 posDifference = BattleSystem.GetClosestEnemy().transform.position - BattleSystem.Party[0].transform.position;
                int x = posDifference.x > 0 ? 0 : (size - 1);  //Get left-most side

                var squares = new BattleSquare[partySize];  //Define array based on 'partySize'
                for (int i = 0; i < partySize; i++)
                {
                    var sq = GridObjs[x, (y - i) < 0 ? (size - 1) : y - i]; // Pull next square

                    #region Skip non-inhabitable squares
                    while (sq == null)
                    {    //Is a square there? (null = non-inhabitable)
                        if ((y - 1) < 0)
                        {  //Are we at bottom?
                            y = (((size + partySize) / 3) + 1);  //If so, center y...
                            x = posDifference.x > 0 ? (x + 1) : (x - 1);    //...and go no next column
                        }

                        y--;    //We can go down
                        sq = GridObjs[x, y];    //Check next square
                        if (sq != null && squares.Contains(sq))
                            sq = null;  //Square is already assigned to unit
                    }
                    #endregion

                    squares[i] = sq;    //Set available square
                }

                //Returns squares found to be used as starting positions for party.
                return squares;
            }

            /**
             * Gets all squares in radius around center point, within
             * specified range.
             * 
             * @return BattleSquare[]
             */
            public static BattleSquare[] SquareVicinity(BattleSquare square, int range)
            {
                Vector2[] radial = new Vector2[] {  //Define radius
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(-1, -1),  new Vector2(-1, 0),  new Vector2(-1, 1),
                };

                List<BattleSquare> sqList = new List<BattleSquare>();
                for (int i = 1; i <= range; i++)
                {
                    foreach (Vector2 v2 in radial)
                    {
                        var sq = GetSquareAtCoordinate(Coordinates(square) + (v2 * i));
                        if (!sq) continue;  //No square at location

                        sqList.Add(sq); //Add sqaure that's within range
                    }
                }

                return sqList.ToArray();
            }

            public static Vector2 Coordinates(BattleSquare square)
            {
                if (!square) return Vector2.zero; //<-- dumb shit

                for (int x = 0; x < GridObjs.GetLength(0); x++)
                    for (int y = 0; y < GridObjs.GetLength(1); y++)
                        if (GridObjs[x, y] && GridObjs[x, y].Equals(square))
                            return new Vector2(x, y);


                return Vector2.zero;
            }

            public static BattleSquare GetSquareAtLocation(Vector2 location) {

                //No grid instantiated
                if(GridObjs == null) {
                    Debug.LogError("No grid instantiated. Returning null.");
                    return null;
                }

                //Check for initial square
                var sq = ReturnFirstSquare();
                if (!sq) {
                    Debug.LogError("No initial square instantiated. Returning null.");
                    return null;
                }

                //Abs location as V2Int
                Vector2Int loc = new Vector2Int(
                    Mathf.Abs(Mathf.FloorToInt(location.x)), Mathf.Abs(Mathf.FloorToInt(location.y)));

                //Abs intial square as V2Int
                Vector2Int firstLoc = new Vector2Int(
                    Mathf.Abs(Mathf.FloorToInt(sq.transform.position.x)), Mathf.Abs(Mathf.FloorToInt(sq.transform.position.y)));

                int x = Mathf.Clamp(Mathf.Abs(loc.x - firstLoc.x), 0, GridObjs.GetLength(0) - 1);
                int y = Mathf.Clamp(Mathf.Abs(loc.y - firstLoc.y), 0, GridObjs.GetLength(1) - 1);

                Debug.Log($"[GetSquareAtLocation] " +
                    $"\nLoc: {loc}" +
                    $"\nFirstLoc: {firstLoc}" +
                    $"\nx: {x} y: {y}" +
                    $"\nGridObjsLength: {GridObjs.GetLength(0)}");

                return GridObjs[x, y];
            }

            private static BattleSquare ReturnFirstSquare()
            {
                for (int y = 0; y < GridObjs.GetLength(1); y++)
                {
                    for (int x = 0; x < GridObjs.GetLength(0); x++)
                    {
                        if (GridObjs[x, y])
                            return GridObjs[x, y];
                    }
                }

                return null;
            }

            private static BattleSquare InstantitateSquare(Vector2 location)
            {
                if (!Parent) Parent = new GameObject("[BATTLE] BattleGrid");

                var newSq = Object.Instantiate(SquarePrefab, Parent.transform);
                newSq.transform.position = new Vector2(Mathf.Floor(location.x) + 0.5f, Mathf.Floor(location.y) + 0.5f);

                var bsComponent = newSq.GetComponent<BattleSquare>();

                return bsComponent;
            }

            public static void ResetGrid()
            {
                SetGridState(BattleSquare.BattleSquareState.Open);
            }

            private static bool ValidSquareLocation(Vector2 location)
            {
                Collider2D[] hit2Ds = Physics2D.OverlapCircleAll(location, 0);

                foreach (Collider2D hit in hit2Ds)
                    if (hit.gameObject.layer == 3) return false;

                if (!ReturnFirstSquare()) return true;
                
                Vector2[] radial = new Vector2[] {
                    Vector2.left, Vector2.right, Vector2.up, Vector2.down,
                    (Vector2.left + Vector2.up), (Vector2.right + Vector2.up),
                    (Vector2.left + Vector2.down), (Vector2.right + Vector2.down)
                };

                foreach (Vector2 v2 in radial) { 
                    if(GetSquareAtLocation(location + v2)) 
                        return true;
                }

                return false;
            }

        }

        static class BattleEvent
        {

            #region Battle Events
            public delegate void PartyMemberChange();
            public static event PartyMemberChange PartyMemberChangeEvent;
            public static void InvokePartyMemberChangeEvent() => PartyMemberChangeEvent?.Invoke();

            public delegate void TakenDamage();
            public static event TakenDamage TakenDamageEvent;
            public static void InvokeTakenDamageEvent() => TakenDamageEvent?.Invoke();

            public delegate void TakenHeal();
            public static event TakenHeal TakenHealEvent;

            public static void InvokeTakenHealEvent() => TakenHealEvent?.Invoke();
            #endregion

            #region Grid Events
            public delegate void SquareCall();
            public static event SquareCall SquareCallEvent;
            public static void InvokeSquareCallEvent()
            {
                SquareCallEvent?.Invoke();
            }
            #endregion
        }

        [System.Serializable]
        public struct Pattern
        {
            public Vector2[] sequence;
            public BattleSquare.BattleSquareState[] states;
            [Min(1)] public int iterations;

            public Pattern(Vector2[] sequence, BattleSquare.BattleSquareState[] states, int iterations)
            {
                this.sequence = sequence;
                this.states = states;
                this.iterations = iterations;
            }

            public bool StateMatch(BattleSquare battleSquare)
            {
                foreach (BattleSquare.BattleSquareState st in states)
                {
                    if (st == battleSquare.state) return true;
                }

                return false;
            }

            public bool StateMatch(BattleSquare.BattleSquareState state)
            {
                foreach (BattleSquare.BattleSquareState st in states)
                {
                    if (st == state) return true;
                }

                return false;
            }

            public override string ToString()
            {
                if (sequence == null || sequence.Length == 0) return "{Empty}";
                string response = "";
                foreach (Vector2 vector in sequence)
                    response += $"{vector} >> ";

                return response;
            }
        }

        [System.Serializable]
        public enum BattleState
        {
            InActive,
            Active,
            EnemyWin,
            PartyWin
        }

    }

        static class Parse
        {

            public static float ParseFloat(string value)
            {
                if (float.TryParse(value, out float x))
                    return x;

                Debug.LogError($"Fail parse value '{value}' expected float.");
                return 0;
            }

            public static int ParseInt(string value)
            {
                if (int.TryParse(value, out int x))
                    return x;

                Debug.LogError($"Fail parse value '{value}' expected int.");
                return 0;
            }

            public static bool Vector2Approx(Vector2 v1, Vector2 v2, Vector2 maxOffset) {
                return Math.Abs(v1.x - v2.x) <= maxOffset.x &&
                    Math.Abs(v1.y - v2.y) <= maxOffset.y;
            }

            public static bool FloatApprox(float a, float b, float offset) {
            return Math.Abs(a - b) <= offset;
            }

            public static Elemental[] ParseDamage(string value, BattleSquare caster, BattleSquare target)
            {
                value = string.Concat(value.Where(c => !char.IsWhiteSpace(c)));
                string[] elements = value.Split(new char[] { '/', ' ' });

                Debug.Log($"Caster: {(caster == null ? "NULL" : caster)} | Target: {(target == null ? "NULL" : target)}");

                Elemental[] casterEnhancement = caster.GetInhabitedUnits()[0].combatant.enhancement;
                Elemental[] targetResistance = target.GetInhabitedUnits()[0].combatant.resitance;

                Elemental[] damage = Elemental.NewElementalTable();
                Elemental x;

                foreach (string e in elements)
                {
                    if (string.IsNullOrEmpty(e)) continue;
                    switch (e.Substring(0, 3).ToLower())
                    {
                        case "tru":
                            x = Elemental.Get(ElementalType.True, damage);
                            x.value = ParseInt(e.Substring(3));
                            Debug.Log(x);
                            break;
                        case "rot":
                            x = Elemental.Get(ElementalType.Rot, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Rot, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Rot, targetResistance);
                            break;
                        case "lif":
                            x = Elemental.Get(ElementalType.Life, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Life, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Life, targetResistance);
                            break;
                        case "str":
                            x = Elemental.Get(ElementalType.Strength, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Strength, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Strength, targetResistance);
                            break;
                        case "rng":
                            x = Elemental.Get(ElementalType.Range, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Range, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Range, targetResistance);
                            break;
                        case "fir":
                            x = Elemental.Get(ElementalType.Fire, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Fire, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Fire, targetResistance);
                            break;
                        case "grd":
                            x = Elemental.Get(ElementalType.Ground, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Ground, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Ground, targetResistance);
                            break;
                        case "ice":
                            x = Elemental.Get(ElementalType.Ice, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Ice, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Ice, targetResistance);
                            break;
                        case "wtr":
                            x = Elemental.Get(ElementalType.Water, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Water, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Water, targetResistance);
                            break;
                        case "elc":
                            x = Elemental.Get(ElementalType.Electric, damage);
                            x.value = ParseInt(e.Substring(3)) + Elemental.Get(ElementalType.Electric, casterEnhancement).value;
                            x = x - Elemental.Get(ElementalType.Electric, targetResistance);
                            break;

                    }
                }

                return damage;
            }

            public static ElementalType ParseActiveElement(string value)
            {

                value = string.Concat(value.Where(c => !char.IsWhiteSpace(c)));
                string[] elements = value.Split(new char[] { '/', ' ' });

                foreach (string e in elements)
                {
                    if (string.IsNullOrEmpty(e)) continue;
                    switch (e[..3].ToLower())
                    {
                        case "tru": return ElementalType.True;
                        case "rot": return ElementalType.Rot;
                        case "lif": return ElementalType.Life;
                        case "str": return ElementalType.Strength;
                        case "rng": return ElementalType.Range;
                        case "fir": return ElementalType.Fire;
                        case "grd": return ElementalType.Ground;
                        case "ice": return ElementalType.Ice;
                        case "wtr": return ElementalType.Water;
                        case "elc": return ElementalType.Electric;
                    }
                }

                Debug.LogWarning($"No elements were found in string: \n{value}");
                return ElementalType.True;
            }

            public static GameObject ParseGameObject(string value)
            {

                GameObject obj = Resources.Load<GameObject>(value);
                if (obj == null) Debug.LogError($"Fail parse value '{value}' expected GameObject.");
                return obj;
            }

            public static ScriptableObject ParseScriptableObject(string value)
            {
                ScriptableObject obj = Resources.Load<ScriptableObject>(value);
                if (obj == null) Debug.LogError($"Fail parse value '{value}' expected ScriptableObject.");
                return obj;
            }

        }

        /*===============================================
         *================ INPUT MANAGER ================
         *===============================================
         *  Responsible for centralizing input functionality.
         *  From here other classes can access InputActions
         *  through 'getter' methods.
         */
        static class InputManager
        {
            // Static reference to InputMaster class, where
            // all inputs are defined.
            private static InputMaster Master = new InputMaster();


            public static InputMaster.PlayerActions PlayerInput() => Master.Player;

            public static InputMaster.CameraActions CameraInput() => Master.Camera;

            public static InputMaster.BattleActions BattleInput() => Master.Battle;
            
        }
}
