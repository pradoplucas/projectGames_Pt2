using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (CharacterController))]

public class scriptPac : MonoBehaviour
{

    //Para controlar a câmera que vai
    //ficar "seguindo" o player..
    GameObject cameraPlayer;

    //Um vetor de 3 posições que vai armazenar
    //a direção do movimento do player,
    //inicia com zero pra não andar sozinho..
    Vector3 moveDirection = Vector3.zero;

    //Variável para manipular o characterController..
    CharacterController controller;

    //Para manipular a rotação da câmera..
    float rotX = 0f, rotY = 0f;

    //Velocidade com que o player irá se mover, 
    //sensibilidade do mouse e um fator para
    //o player se abaixar e levantar de maneira
    //mais suave..
    private float speed, sensitivity;

    public Text pontuacao;
    private int pontos = 0;

    public Image vida1, vida2, vida3;
    private int vida = 3;

    private bool fullPilula = false, fullPowerUp = false;
    private int countPilula = 0, countPowerUp = 0;

    public GameObject pilulasPrefab;

    private bool respawnPac = false, isPortal = false;

    public int velGhosts = 1;

    public bool[] isPower = new bool[4];

    private AudioSource audioSource;
    public AudioClip[] som = new AudioClip[3];
    public Light areaLight;

    private bool isLight = false;

    // Start is called before the first frame update
    void Start(){
        //Armazena na variável camera uma câmera
        //que está como componente filho de player..
        cameraPlayer = GetComponentInChildren (typeof (Camera)).transform.gameObject;

        //Coloca a camera na altura da cabeça de player..
        cameraPlayer.transform.localPosition = new Vector3 (0, 0.2f, 0);

        //Camera começa com rotação 0, 0, 0..
        cameraPlayer.transform.localRotation = Quaternion.identity;
        //cameraPlayer.transform.localEulerAngles = new Vector3(10, 210, 0);

        speed = 3f;
        sensitivity = 1000f;

        //Para indicar que é um GameObject 
        //do tipo player, facilita quando
        //a aplicação é grande..
        transform.tag = "Player";

        //Controller recebe o componente 
        //CharacterController..
        controller = GetComponent<CharacterController> ();

        pontuacao.enabled = true;
        vida1.enabled = true;
        vida2.enabled = true;
        vida3.enabled = true;

        isPower[0] = false;
        isPower[1] = false;
        isPower[2] = false;
        isPower[3] = false;

        audioSource = GetComponent<AudioSource>();
        areaLight.enabled = false;

        Instantiate(pilulasPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        //Deixa o cusor escondido quando este está
        //posicionado na tela do game..
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update(){
        
        //Para o player se mover sempre para 
        //onde a câmera está virada quando
        //apertar para frente (W ou SetaUp)..
        Vector3 direcForward = new Vector3 (cameraPlayer.transform.forward.x, 0, cameraPlayer.transform.forward.z);

        //Para o player se mover sempre para 
        //a lateral de onde a câmera está virada
        //quando apertar para o lado (A/D ou <-/->).. 
        Vector3 direcSide = new Vector3 (cameraPlayer.transform.right.x, 0, cameraPlayer.transform.right.z);

        //Normalizar os vetores para que o valor
        //intermediário deles não altere na velocidade..
        direcForward.Normalize ();
        direcSide.Normalize ();

        //As duas variáveis só tem algum valor
        //quando são pressionadas as teclas
        //de movimento, caso contrário, são zero..

        float moveForward = Input.GetAxis ("Vertical");
        float moveSide = Input.GetAxis ("Horizontal");

        direcForward *= moveForward;
        direcSide *= moveSide;

        //A soma dos dois vetores para determinar
        //qual a direção exata que está se movendo..
        Vector3 direcFinal = direcForward + direcSide;

        //Normalizar a direção final..
        //Quando eu tenho algum movimento,
        //pra qualquer lado, direcFinal vai
        //ser ou menor que -1 ou maior que +1,
        //elevando ao quadrado, esse valor é maior
        //que 1, sendo assim, eu normalizo ele em 1,
        //pois eu que isso seja só uma direção, e não
        //um fator de velocidade na hora de multiplicar
        //lá na frente..
        if (direcFinal.sqrMagnitude > 1) {
            direcFinal.Normalize ();
        }

        //Determina as direções..
        moveDirection = new Vector3 (direcFinal.x, direcFinal.y, direcFinal.z) * speed * Time.deltaTime;

        //Faz o player se mover..
        controller.Move (moveDirection);

        pontuacao.text = pontos.ToString();

        //Chama os métodos necessários..
        MoveCamera (); 

        Restart();

        if(respawnPac){ 
            transform.position = new Vector3(0, 0.5f, -5.1f);
            respawnPac = false;
        }

        if(isPortal){ 
            Vector3 positionPac = transform.position;
            float posAuxPortal = 0;

            if(transform.position.x > 0) posAuxPortal = transform.position.x - 0.6f;
            else if(transform.position.x < 0) posAuxPortal = transform.position.x + 0.6f;

            transform.position = new Vector3 (-posAuxPortal, transform.position.y, transform.position.z);

            isPortal = false;
        }

        if(isLight){
            if(Input.GetKeyDown(KeyCode.E)){
                areaLight.enabled = !areaLight.enabled;
            }
        }
    }

    //Função para mover a câmera..
    void MoveCamera () {
        //Determina as rotações em x e y..
        rotX += Input.GetAxis ("Mouse X");
        rotY += Input.GetAxis ("Mouse Y");

        rotX = ClampCamera (rotX, -360, 360);
        rotY = ClampCamera (rotY, -80, 85);

        Quaternion xQuat = Quaternion.AngleAxis (rotX, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis (rotY, -Vector3.right);

        Quaternion rotFinal = Quaternion.identity * xQuat * yQuat;
        cameraPlayer.transform.localRotation = Quaternion.Lerp (cameraPlayer.transform.localRotation, rotFinal, Time.deltaTime * sensitivity);
    }

    //Função para impedir que o angulo 
    //esteja fora dos limites..
    float ClampCamera (float angle, float min, float max) {
        if (angle < -360) {
            angle = 0;
        }
        if (angle > 360) {
            angle = 0;
        }

        return Mathf.Clamp (angle, min, max);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Ghost") && !other.gameObject.GetComponent<scriptGhost>().isEscape){
            vida--;

            if(vida == 2){
                vida1.enabled = false;
                respawnPac = true;
            }

            else if(vida == 1){
                vida2.enabled = false;
                respawnPac = true;
            }

            else if(vida == 0){
                vida3.enabled = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
                //Destroy(gameObject);
            }
        } 

        else if(other.CompareTag("Pilula")){
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            pontos++;
            countPilula++;
            audioSource.PlayOneShot(som[0]);

            //Debug.Log("Pilula: " + countPilula);

            if(countPilula == 98){
                fullPilula = true;
                countPilula = 0;
            }
        }

        else if(other.CompareTag("PowerUp")){
            other.gameObject.SetActive(false);
            pontos+=10;
            countPowerUp++;
            audioSource.PlayOneShot(som[1]);
            audioSource.PlayOneShot(som[2]);

            isPower[0] = true;
            isPower[1] = true;
            isPower[2] = true;
            isPower[3] = true;

            Invoke("isPowerFalse", 10f);

            //Debug.Log("Power: " + countPowerUp);

            if(countPowerUp == 4){
                fullPowerUp = true;
                countPowerUp = 0;
            }
        }

        else if(other.CompareTag("Portal")){
            isPortal = true;
        }

        else if(other.CompareTag("Light")){
            isLight = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Light")){
            isLight = false;
        }
    }

    private void isPowerFalse(){
        isPower[0] = false;
        isPower[1] = false;
        isPower[2] = false;
        isPower[3] = false;
    }

    private void Restart(){
    
        if(fullPilula && fullPowerUp){
            GameObject auxPilula = GameObject.FindWithTag ("PilulaPrefab");
            Destroy(auxPilula.gameObject);

            Instantiate(pilulasPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            fullPilula = false;
            fullPowerUp = false;
            velGhosts++;
        }
    }
}
