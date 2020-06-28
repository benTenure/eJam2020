using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NYEBallController : MonoBehaviour
{
    // Variables for dealing with lerping colors
    public Material glowingFacesMaterial;
    
    private Color[] _cycledColors = {Color.red, Color.magenta, Color.yellow, Color.blue, Color.green};
    private Color _lerpedColor;

    private int _first = 0;
    private int _second = 1;

    private readonly float _maximumTime = 1.0f;
    private readonly float _lerpSpeed = 0.5f;
    
    private static float _t = 0.0f;
    
    // Variables for dealing with lerping vectors
    public GameObject finalLocation;
    private bool _rollToFinish = false;
    private Vector3 _startPosition;
    private Vector3 _finalPosition;
    private float _startTime;
    private float _journeyLength;
    private float _vectorLerpSpeed = 20.0f;

    // Start is called before the first frame update
    private void Start()
    {
        _lerpedColor = _cycledColors[0];
        _startPosition = transform.position;
        _finalPosition = finalLocation.transform.position;
        _journeyLength = Vector3.Distance(_startPosition, _finalPosition);
    }

    // Update is called once per frame
    private void Update()
    {
        // Handling the ball's changing colors (emissive too!)
        _lerpedColor = Color.Lerp(_cycledColors[_first], _cycledColors[_second], _t);
        
        glowingFacesMaterial.SetColor("_Color", _lerpedColor);
        glowingFacesMaterial.SetColor("_Emission", _lerpedColor);
        
        _t += _lerpSpeed * Time.deltaTime;

        if (_t >= _maximumTime)
        {
            _first = IncrementCounter(_first);
            _second = IncrementCounter(_second);
            _t = 0.0f;
        }
        
        // Handling the ball's movement offscreen when player dies
        if (!_rollToFinish) 
            return;

        var distanceCovered = (Time.time - _startTime) * _vectorLerpSpeed;
        var fractionOfJourney = distanceCovered / _journeyLength;
        transform.position = Vector3.Lerp(_startPosition, _finalPosition, fractionOfJourney);

        if (fractionOfJourney >= 1f) 
            _rollToFinish = false;
    }

    private int IncrementCounter(int current)
    {
        if (current >= _cycledColors.Length - 1) 
            return 0;
        
        return ++current;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;
        
        _rollToFinish = true;
        _startTime = Time.time;
        other.gameObject.SetActive(false);
        GameManagerScript.Instance.PlayerDied();
        GameObject.FindObjectOfType<WorldGeneration>()?.SetScrollModifier(0);
    }
}
