using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(PlatformerCharacter2D))]
    public class Platformer2DUserControl : PunBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;

        //Healthbar image
        public Image HPbarImage;
        // Arm rotation Stuff
        public int rotationOffset = 0;
        private GameObject arm;

        //Weapon Stuff
        public float fireRate = 0f;
        public int Damage = 10;
        public LayerMask whatToHit;
        public Transform BulletTrailPrefab;
        public float effectSpawnRate = 10f;
        public Transform MuzzleFlashPrefab;
        public Text Score, AmmoText, KillText;

        private bool isReloading = false;

        private float timeToFire = 0;
        private Transform firePoint;
        private float timeToSpawnEffect = 0f;

        [SerializeField]
        private float Health, maxHealth;
        [SerializeField]
        private int Ammo,Deaths,maxAmmo,Kills;
        [SerializeField]
        private Quaternion ArmTransform;

        private myNetworkManager NetworkManager;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            //Getting the Arm
            arm = gameObject.transform.GetChild(3).gameObject;
            //Getting the Firepoint
            this.firePoint = gameObject.transform.GetChild(3).GetChild(0).GetChild(0);/*transform.Find ("Fire Point");*/
            if (this.firePoint == null)
            {
                Debug.LogError("No Fire Point?  WHAT?!");
            }
        }

        private void Start()
        {

            maxHealth = Health;
            HPbarImage.fillAmount = Health / maxHealth;
            NetworkManager = FindObjectOfType<myNetworkManager>();
            if (photonView.isMine)
            {
                Score = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
                AmmoText = GameObject.FindGameObjectWithTag("Ammo").GetComponent<Text>();
                maxAmmo = 35;
                Ammo = maxAmmo;
                AmmoText.text = Ammo.ToString();
            }

        }

        private void Update()
        {
            if (!photonView.isMine)
            {
                return;
            }
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            ArmRotation();
            Weapon();

        }


        private void FixedUpdate()
        {
            if (!photonView.isMine)
            {
                return;
            }
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }

        private void ArmRotation()
        {
            // subtracting the position of the player from the mouse position
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize(); // normalizing the vector.  Meaning that all sums of the vector will be equal to 1.

            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;   // find the angle in degrees
            arm.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + this.rotationOffset);
            //ArmTransform = arm.transform.rotation;
        }

        // All the weapon Related stuff

        private void Weapon()
        {
            if (isReloading == true)
            {
                return;
            }
                if (Ammo > 0)
                {
                    if (this.fireRate == 0)
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            photonView.RPC("Shoot", PhotonTargets.All);
                            Ammo--;
                            AmmoText.text = Ammo.ToString();
                        }
                    }
                    else
                    {
                        if (Input.GetButton("Fire1") && Time.time > this.timeToFire)
                        {
                            this.timeToFire = Time.time + 1 / this.fireRate;
                            photonView.RPC("Shoot", PhotonTargets.All);
                            Ammo--;
                            AmmoText.text = Ammo.ToString();
                        }
                    }
                }
                else
                {
                    if (Ammo == 0 && isReloading == false)
                    {
                        isReloading = true;
                        Invoke("Reloading", 1.5f);
                    }
                }
            }

        private void Reloading()
        {
            Ammo = maxAmmo;
            AmmoText.text = Ammo.ToString();
            isReloading = false;
        }
        [PunRPC]
        private void Shoot()
        {
            GameObject enemy;
            Platformer2DUserControl enemyController;
            Vector2 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition = new Vector2(screenToWorldPoint.x, screenToWorldPoint.y);
            Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
            RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatToHit);
            if (Time.time >= this.timeToSpawnEffect)
            {
                this.Effect();
                this.timeToSpawnEffect = Time.time + 1 / this.effectSpawnRate;
            }
            Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);
            if (hit.collider != null)
            {
                Debug.DrawLine(firePointPosition, hit.point, Color.red);
                Debug.Log("We hit " + hit.collider.name + " and did " + this.Damage + " damage.");
                if (hit.collider.tag == "Player" && photonView.isMine)
                {
                    enemy = hit.collider.gameObject;    
                    enemyController = enemy.GetComponent<Platformer2DUserControl>();
                    //enemyController.Damaged(this.Damage);
                    enemyController.photonView.RPC("Damaged", PhotonTargets.All, this.Damage, this.gameObject);
                    //photonView.RPC("Damaged", PhotonTargets.All);
                    //Debug.Log(enemyController.Health);
                    //if (enemyHP <= 0f)
                    //{
                    //    Debug.Log("Enemy is Dead");
                    //}
                }
            }
        }

        [PunRPC]
        public void Damaged(int enemyDamage,GameObject shooter)
        {
            Platformer2DUserControl shooterController;
            shooterController = shooter.GetComponent<Platformer2DUserControl>();
            Health -= enemyDamage;
            HPbarImage.fillAmount = Health / maxHealth;
            if (Health <= 0)
            {
                transform.position = NetworkManager.spawns[PhotonNetwork.player.ID - 1].transform.position;
                Health = maxHealth;
                HPbarImage.fillAmount = Health / maxHealth;
                shooterController.photonView.RPC("GotAKill", PhotonTargets.All);

                if (photonView.isMine)
                {
                    Deaths++;
                    Score.text = Deaths.ToString();
                }
            }
        }

        [PunRPC]
        public void GotAKill()
        {
            Kills++;
            KillText.text = Kills.ToString();
        }

        private void Effect()
        {
            Instantiate(this.BulletTrailPrefab, this.firePoint.position, firePoint.rotation);
            Transform clone = (Transform)Instantiate(this.MuzzleFlashPrefab, this.firePoint.position, firePoint.rotation);
            clone.parent = this.firePoint;
            float size = UnityEngine.Random.Range(0.6f, 0.9f);
            clone.localScale = new Vector3(size, size, size);
            Destroy(clone.gameObject, 0.02f);
        }



        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                //// We own this player: send the others our data
                stream.SendNext(Health);
                stream.SendNext(Ammo);
                stream.SendNext(Kills);
               // stream.SendNext(ArmTransform);
            }
            else
            {
                //// Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.Ammo = (int)stream.ReceiveNext();
                this.Kills = (int)stream.ReceiveNext();
                //this.ArmTransform = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
