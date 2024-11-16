using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    Rigidbody rbody;
    public float speed;
    public float mouseSensitivity;
    public Vector3 forwardV = new Vector3(0, 0, 1);
    public Vector3 rightV = new Vector3(1, 0, 0);
    public float twistAngle, pitchAngle;
    public GameObject firstBullet; 
    public GameObject bulletRef;   
    public KeyCode fireKey;    
    public Vector3 bulletStart;   

    public float coolDown = 0f; 
    public float coolTime = 0.5f; 

    public Text scoreText;
    public AudioSource balloonPopSound; 
    public AudioSource fireSound; 
    public int score = 0; 



    Vector3 initialPosition;
    Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        twistAngle = transform.rotation.eulerAngles.y;
        pitchAngle = transform.rotation.eulerAngles.x;
        forwardV = Quaternion.Euler(0, -twistAngle, 0) * forwardV;
        rightV = Quaternion.Euler(0, -twistAngle, 0) * rightV;
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        bulletStart=firstBullet.transform.position- transform.position;

        Destroy(firstBullet);
            }  


    // Update is called once per frame
    void Update()
    {
        // Reduce coolDown over time
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }


        KeyMove();
        if (Input.GetMouseButton(0))
            MouseRotate();
        else
            rbody.angularVelocity = -0.5f * rbody.angularVelocity;
   
         if (Input.GetKey(fireKey)) { 
         Fire();
         PlayFireSound();
        }
    }   

    void KeyMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 dir = forwardV * horizontalInput * speed + rightV * verticalInput * speed;
        rbody.velocity = new Vector3(dir.z, 0, dir.x);
    }

    void MouseRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 || mouseY != 0)
        {
            float twistInput = -mouseX * mouseSensitivity;
            float pitchInput = -mouseY * mouseSensitivity;
            twistAngle += twistInput;
            pitchAngle += pitchInput;

            //transform.Rotate(pitchInput, 0, 0);
            //transform.Rotate(0, twistInput, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Rotate(0, twistAngle, 0);

            if (pitchAngle < -60)
                pitchAngle = -60;
            else if (pitchAngle > 180 && pitchAngle < 300)
                pitchAngle = -60;
            else if (pitchAngle > 60 && pitchAngle <= 180)
                pitchAngle = 60;
            transform.Rotate(pitchAngle, 0, 0);

            forwardV = Quaternion.Euler(0, -twistInput, 0) * forwardV;
            rightV = Quaternion.Euler(0, -twistInput, 0) * rightV;
        }
    }

    float vx()
    {
        return rbody.velocity.x;
    }
    float vy()
    {
        return rbody.velocity.y;
    }
    float vz()
    {
        return rbody.velocity.z;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rbody.velocity = new Vector3(0, 0, 0);
        rbody.angularVelocity = new Vector3(0, 0, 0);
    }
    void Fire()
    {
        if (coolDown <= 0)
        {
            GameObject bullet = Instantiate(bulletRef, transform.position + bulletStart, Quaternion.identity);
            bullet.transform.rotation = Quaternion.Euler(90, twistAngle, 0);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            float bulletSpeed = 20f; // Adjust this speed as needed
            bulletRigidbody.velocity = transform.forward * bulletSpeed;
            coolDown = coolTime;
        }
    }
    private void PlayFireSound()
    {
        if (fireSound != null && !fireSound.isPlaying)
        {
            fireSound.Play();
        }
    }

    }
