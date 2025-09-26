using UnityEngine;
public class PressurePlate : MonoBehaviour
{
    [Tooltip("door controller que se abre/cierra al pisar la placa")]
    public DoorController door;

    
    int pressers = 0;

    void Reset()
    {
       
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsValidPresser(other))
        {
            pressers++;
            if (pressers == 1)
            {
                
                if (door != null) door.Open();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidPresser(other))
        {
            pressers = Mathf.Max(0, pressers - 1);
            if (pressers == 0)
            {
                
                if (door != null) door.Close();
            }
        }
    }

    bool IsValidPresser(Collider other)
    {
        if (other == null) return false;

        
        if (other.CompareTag("Enemy")) return true;

        
        //if (other.GetComponent<PatrolEnemy>() != null) return true;

        
        return false;
    }
}
