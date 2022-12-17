using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(EnemyUnit))]
public class EnemyAI : MonoBehaviour
{
    public IMovement Movement = new();
    public IAttack Attack = new();

    [SerializeField]private ModuleTable[] ModuleTables;

    public bool waiting { get; private set; }


    private void Start() {
        Movement.SetEnemyUnit(GetComponent<EnemyUnit>());
        Attack.SetEnemyUnit(GetComponent<EnemyUnit>());
    }

    public void NextMove() {
        ModuleTables = GetModuleTables();

        Debug.Log($"[EnemyAI] '{name}' Table: \n{ModuleTable.ToString(ModuleTables)}");
        if (NullConfidence(ModuleTables)) return;

        Debug.Log($"[EnemyAI] Performing {ModuleTables[0].Module.GetType().Name} for unit '{name}'");
        ModuleTables[0].Module.PerformAction();
    }

    public void GenerateInitialModuleTable() => ModuleTables = GetModuleTables();
    private ModuleTable[] GetModuleTables() {
        ModuleTable[] table = new ModuleTable[] {
            new ModuleTable(Movement.Confidence(), Movement), 
            new ModuleTable(Attack.Confidence(), Attack)
        };
        return table.OrderByDescending(e => e.Confidence).ToArray();
    }

    public bool NullConfidence() => NullConfidence(ModuleTables);

    private bool NullConfidence(ModuleTable[] table) => table[0].Confidence == 0;

    protected class ModuleTable {
        public float Confidence;
        public IModule Module;

        public ModuleTable(float confidence, IModule module) {
            Confidence = confidence;
            Module = module;
        }

        public static string ToString(ModuleTable[] tables) {
            if (tables == null) return "<EMPTY>";

            string result = string.Empty;
            tables.ToList().ForEach(t => result += ($"{t.Module.GetType().Name} | {t.Confidence}\n"));
            return result;
        }
    }
}
