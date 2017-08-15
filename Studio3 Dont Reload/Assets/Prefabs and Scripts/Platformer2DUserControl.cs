using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

    [RequireComponent(typeof(PlatformerCharacter2D))]
    public class Platformer2DUserControl : PunBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;

        //Healthbar image
        public Image HPbarImage;
        // Arm rotation Stuff
        public int rotationOffset = 0;
        private GameObject arm,endgameCanvas;

        //Weapon Stuff
        public float fireRate = 0f;
        public int Damage = 10;
        public LayerMask whatToHit;
        public Transform BulletTrailPrefab,BulletTrailPrefab2,BulletTrailPrefab3,CurrBulletPrefab;
        public float effectSpawnRate = 10f;
        public Transform MuzzleFlashPrefab;
        public Text Score, AmmoText, KillText;

        private bool isReloading = false;
        private bool isInvincible = false;
        private bool isEndGame = false;

        private float timeToFire = 0;
        private Transform firePoint;
        private float timeToSpawnEffect = 0f;

        private GameObject myManager;

        //public Sprite myDefaultGun,Gun2,Gun3;
        public Sprite[] Guns;

        private SpriteRenderer myGunRenderer;
        
        [SerializeField]
        private float Health, maxHealth,reloadTimer,invincibilityTimer;
        [SerializeField]
        private int Ammo,Deaths,maxAmmo,Kills,myID,myCurrGun;
        [SerializeField]
        private Quaternion ArmTransform;

        private myNetworkManager NetworkManager;

        [SerializeField]
        private GameObject[] players, gunPictures;
        public int[] playerIDs;
        [SerializeField]
        private GameObject gunPictures1, gunPictures2, gunPictures3, gunPictures4;

        public AudioSource myAudioSource;
        private AudioClip myCurrGunSound;
        public AudioClip[] mySounds;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            //Getting the Arm
            arm = gameObject.transform.GetChild(3).gameObject;
            endgameCanvas = GameObject.FindGameObjectWithTag("EndGame");
            //Getting the Firepoint
            this.firePoint = gameObject.transform.GetChild(3).GetChild(0).GetChild(0);/*transform.Find ("Fire Point");*/
            if (this.firePoint == null)
            {
                Debug.LogError("No Fire Point?  WHAT?!");
            }
            myGunRenderer = gameObject.transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>();
            Guns[0] = myGunRenderer.sprite;
            invincibilityTimer = 2.0f;
        }

        private void Start()
        {
            myCurrGun = 0;
            playerIDs = new int[2];
            reloadTimer = 1.5f;
            gunPictures = new GameObject[4];
            maxHealth = Health;
            HPbarImage.fillAmount = Health / maxHealth;
            NetworkManager = FindObjectOfType<myNetworkManager>();

            myAudioSource = gameObject.GetComponent<AudioSource>();
            myCurrGunSound = mySounds[1];
            CurrBulletPrefab = BulletTrailPrefab;
            if (photonView.isMine)
            {
                this.myID = PhotonNetwork.player.ID;
                Score = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
                AmmoText = GameObject.FindGameObjectWithTag("Ammo").GetComponent<Text>();
                KillText = GameObject.FindGameObjectWithTag("Kills").GetComponent<Text>();
                gunPictures1 = GameObject.FindGameObjectWithTag("gunImage1");
                gunPictures2 = GameObject.FindGameObjectWithTag("gunImage2");
                gunPictures3 = GameObject.FindGameObjectWithTag("gunImage3");
                //gunPictures4 = GameObject.FindGameObjectWithTag("gunImage4");
                //gunPictures = GameObject.FindGameObjectsWithTag("gunImage1");
                maxAmmo = 35;
                Ammo = maxAmmo;
                AmmoText.text = Ammo.ToString();
                Invoke("GettingPlayers", 3f);
                Invoke("GettingIDs", 4f);

                myManager = GameObject.FindGameObjectWithTag("Manager");

                gunPictures1.transform.GetChild(0).gameObject.SetActive(false);

            }

        }

        private void Update()
        {
        //if (Input.GetKeyUp(KeyCode.F))
        //{
        //    myGunRenderer.sprite = Guns[2];
        //}
        if (isEndGame)
        {
            return;
        }
            HPbarImage.fillAmount = Health / maxHealth;
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
        if (isEndGame)
        {
            return;
        }
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
                        Invoke("Reloading", reloadTimer);
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
                    enemyController.photonView.RPC("Damaged", PhotonTargets.All, this.Damage,myID);
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
        public void Damaged(int enemyDamage,int enemyID)
        {
            if(isInvincible == true)
        {
            Debug.Log("Target is invincible for ");
            return;
        }
            //players = GameObject.FindObjectsOfType<Platformer2DUserControl>();
            //Platformer2DUserControl shooterController;
            Health -= enemyDamage;
            //HPbarImage.fillAmount = Health / maxHealth;
            if (Health <= 0)
            {
                transform.position = NetworkManager.spawns[PhotonNetwork.player.ID - 1].transform.position;
                Health = maxHealth;
                //HPbarImage.fillAmount = Health / maxHealth;
                this.isInvincible = true;
                Invoke("Invincibility", invincibilityTimer);
            //shooterController.photonView.RPC("GotAKill", PhotonTargets.All);
            if (enemyID != 30)
           {
                //if enemy ID is 30 it'll be the enviroment that'll be killing the player
                for (int i = 0; i < playerIDs.Length; i++)
                {
                    if (playerIDs[i] == enemyID)
                    {
                        players[i].GetComponent<Platformer2DUserControl>().photonView.RPC("GotAKill", PhotonTargets.All);
                    }
                }
            }
                if (photonView.isMine)
                {
                    Deaths++;
                    Score.text = Deaths.ToString();
                    if (myCurrGun > 0)
                    {
                        myCurrGun--;
                        photonView.RPC("GunUpgrade", PhotonTargets.All,myCurrGun);
                    }
                }
            }
        }

        [PunRPC]
        public void GotAKill()
        {
            if (!photonView.isMine)
            {
                return;
            }
            Kills++;
            KillText.text = Kills.ToString();
            if(myCurrGun < 2)
            {
                myCurrGun++;
                photonView.RPC("GunUpgrade", PhotonTargets.All,myCurrGun);
            }
            if(Kills >= 15)
        {
            photonView.RPC("endGame", PhotonTargets.All);
        }
        }

    [PunRPC]
        private void endGame()
    {
        isEndGame = true;
        if (Kills >= 15)
        {
            endgameCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            endgameCanvas.transform.GetChild(0).gameObject.SetActive(true);
        }

        Invoke("PartOfEndGame", 3f);

    }

        private void Effect()
        {
            myAudioSource.PlayOneShot(myCurrGunSound);
            Instantiate(this.CurrBulletPrefab, this.firePoint.position, firePoint.rotation);
            Transform clone = (Transform)Instantiate(this.MuzzleFlashPrefab, this.firePoint.position, firePoint.rotation);
            clone.parent = this.firePoint;
            float size = UnityEngine.Random.Range(0.6f, 0.9f);
            clone.localScale = new Vector3(size, size, size);
            Destroy(clone.gameObject, 0.02f);
        }


        private void GettingIDs()
        {
            for (int i = 0; i < players.Length; i++)
            {
                playerIDs[i] = players[i].GetComponent<Platformer2DUserControl>().myID;
            }
        }

        private void GettingPlayers()
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        [PunRPC]
        private void GunUpgrade(int CurrentGun)
        {
        //for (int i = 0; i < 3; i++)
        //{
        //    gunPictures[i].transform.GetChild(0).gameObject.SetActive(true);
        //}
            myGunRenderer.sprite = Guns[CurrentGun];
            if (CurrentGun == 0)
            {
                this.Damage = 10;
                this.fireRate = 5f;
                this.maxAmmo = 35;
                this.Ammo = maxAmmo;
                this.CurrBulletPrefab = BulletTrailPrefab;
                this.myCurrGunSound = mySounds[1];
                gunPictures1.transform.GetChild(0).gameObject.SetActive(false);
                gunPictures2.transform.GetChild(0).gameObject.SetActive(true);
        }
            if (CurrentGun == 1)
            {
                this.Damage = 4;
                this.fireRate = 14f;
                this.maxAmmo = 100;
                this.Ammo = maxAmmo;
                this.CurrBulletPrefab = BulletTrailPrefab2;
                this.myCurrGunSound = mySounds[2];
                gunPictures2.transform.GetChild(0).gameObject.SetActive(false);
                gunPictures1.transform.GetChild(0).gameObject.SetActive(true);
                gunPictures3.transform.GetChild(0).gameObject.SetActive(true);
        }
            if (CurrentGun == 2)
            {
                this.Damage = 30;
                this.fireRate = 1f;
                this.maxAmmo = 5;
                this.Ammo = maxAmmo;
                this.CurrBulletPrefab = BulletTrailPrefab3;
                this.myCurrGunSound = mySounds[3];
                gunPictures3.transform.GetChild(0).gameObject.SetActive(false);
                gunPictures2.transform.GetChild(0).gameObject.SetActive(true);
        }

        }

        private void Invincibility()
    {
        isInvincible = false;
    }

        private void PartOfEndGame()
    {
        myManager.GetComponent<myNetworkManager>().menuButton();
    }


        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                //// We own this player: send the others our data
                stream.SendNext(Health);
                stream.SendNext(Ammo);
                stream.SendNext(myID);
                stream.SendNext(Damage);
                stream.SendNext(myCurrGun);
                stream.SendNext(isInvincible);
            }
            else
            {
                //// Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.Ammo = (int)stream.ReceiveNext();
                this.myID = (int)stream.ReceiveNext();
                this.Damage = (int)stream.ReceiveNext();
                this.myCurrGun = (int)stream.ReceiveNext();
                this.isInvincible = (bool)stream.ReceiveNext();
            }
        }
 }

