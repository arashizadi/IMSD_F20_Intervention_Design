using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class CubeBehaviour : Bolt.EntityEventListener<ICubeState>
{
    public GameObject[] WeaponObjects;

    private float _resetColorTime;
    private Renderer _renderer;

    public override void Attached()
    {
        _renderer = GetComponent<Renderer>();

        state.SetTransforms(state.CubeTransform, transform);

        if (entity.IsOwner)
        {
            state.CubeColor = new Color(Random.value, Random.value, Random.value);

            // NEW: On the owner, we want to setup the weapons, the Id is set just as the index
            // and the Ammo is randomized between 50 to 100
            for (int i = 0; i < state.WeaponArray.Length; ++i)
            {
                state.WeaponArray[i].WeaponId = i;
                state.WeaponArray[i].WeaponAmmo = Random.Range(50, 100);
            }

            //NEW: by default we don't have any weapon up, so set index to -1
            state.WeaponActiveIndex = -1;
        }

        state.AddCallback("CubeColor", ColorChanged);

        // NEW: we also setup a callback for whenever the index changes
        state.AddCallback("WeaponActiveIndex", WeaponActiveIndexChanged);

        state.Health = 100;
    }

    public override void SimulateOwner()
    {
        var speed = 4f;
        var movement = Vector3.zero;
        var angle = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
        if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
        if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
        if (Input.GetKey(KeyCode.D)) { movement.x += 1; }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                angle.y -= 25;
            else if (Input.GetKey(KeyCode.RightArrow))
                angle.y += 25;
        }
        else
            angle.y = 0;

        // NEW: Input polling for weapon selection
        if (Input.GetKeyDown(KeyCode.Alpha1)) state.WeaponActiveIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) state.WeaponActiveIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) state.WeaponActiveIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha0)) state.WeaponActiveIndex = -1;

        if (movement != Vector3.zero)
        {
            transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
        }
        if (angle != Vector3.zero && movement != Vector3.zero)
            transform.Rotate(angle * speed/3 * BoltNetwork.FrameDeltaTime);
        else if (angle != Vector3.zero)
            transform.Rotate(angle * speed * BoltNetwork.FrameDeltaTime);

        if (Input.GetKeyDown(KeyCode.F))
        {
            var flash = FlashColorEvent.Create(entity);
            flash.FlashColor = Color.red;
            flash.Send();
        }
    }

    public override void OnEvent(FlashColorEvent evnt)
    {
        _resetColorTime = Time.time + 0.2f;
        _renderer.material.color = evnt.FlashColor;
    }

    void Update()
    {
        if (_resetColorTime < Time.time)
        {
            _renderer.material.color = state.CubeColor;
        }
    }

    void OnGUI()
    {
        if (entity.IsOwner)
        {
            GUI.color = state.CubeColor;
            GUILayout.Label("@@@");
            GUI.color = Color.white;
        }
    }

    void ColorChanged()
    {
        GetComponent<Renderer>().material.color = state.CubeColor;
    }

    void WeaponActiveIndexChanged()
    {
        for (int i = 0; i < WeaponObjects.Length; ++i)
        {
            WeaponObjects[i].SetActive(false);
        }

        if (state.WeaponActiveIndex >= 0)
        {
            int objectId = state.WeaponArray[state.WeaponActiveIndex].WeaponId;
            WeaponObjects[objectId].SetActive(true);
        }
    }


}
