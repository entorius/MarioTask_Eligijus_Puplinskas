using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    class FireworkController: MonoBehaviour
    {
        Vector3 lastFireworkPosition;
        public IEnumerator SpawnFireworks(int fireworksNumber, GameObject firework)
        {

            for (int i = 0; i < Mathf.Max(3, fireworksNumber); i++)
            {
                GameObject spawnedFirework = SpawnFirework(firework);
                yield return new WaitForSeconds(2f);
                Destroy(spawnedFirework);
            }

        }
        private GameObject SpawnFirework(GameObject firework)
        {
            GameObject instanciatedFirework = null;
            bool fireworkSpawned = false;
            while (!fireworkSpawned)
            {
                Vector3 fireworkPosition = new Vector3(Random.Range(120f, 130f), Random.Range(2f, 6f));
                instanciatedFirework = Instantiate(firework, fireworkPosition, Quaternion.identity);
                fireworkSpawned = true;
            }
            return instanciatedFirework;
        }
    }
}
