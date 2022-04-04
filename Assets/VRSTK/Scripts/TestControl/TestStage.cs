﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using VRSTK.Scripts.Telemetry;

namespace VRSTK
{
    namespace Scripts
    {
        namespace TestControl
        {
            ///<summary>Defines a stage of an experiment. Each stage has its own user-defined input properties, as well as functions to call when the stage starts.</summary>
            public class TestStage : MonoBehaviour
            {
                ///<summary>Properties which are filled in before the stage starts.</summary>
                public TestControllerProperty[] startProperties;
                ///<summary>Properties which are filled in while the stage is running.</summary>
                public TestControllerProperty[] runningProperties;
                public List<GameObject> GameobjectsToActivate = new List<GameObject>();
                public List<GameObject> GameobjectsToDeactivate = new List<GameObject>();
                public List<GameObject> GameobjectsToSendMessageTo = new List<GameObject>();
                public List<string> messagesToSend = new List<string>();
                public bool hasTimeLimit;
                public int timeLimit;
                public TestController myController;
                public GameObject startButton;
                public GameObject timeText;
                public GameObject propertyParent;
                public GameObject buttonParent;
                private static bool started;
                private Hashtable values = new Hashtable();
                private static float time;


                private void Start()
                {
                    started = false;
                    startProperties = Array.ConvertAll(ArrayTools.ClearNullReferences(startProperties), item => item as TestControllerProperty);
                    runningProperties = Array.ConvertAll(ArrayTools.ClearNullReferences(runningProperties), item => item as TestControllerProperty);
                    foreach (TestControllerProperty p in runningProperties)
                    {
                        p.gameObject.SetActive(false);
                    }
                    if (myController.testStages[0] == gameObject)
                    {
                        startButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Test";
                    }
                    else
                    {
                        startButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Start Stage";
                    }
                }

                void Update()
                {
                    if (started)
                    {
                        time += Time.deltaTime;
                        timeText.GetComponent<Text>().text = Mathf.Round(time).ToString();
                        if (hasTimeLimit && time >= timeLimit)
                        {
                            ToggleTest(startButton);
                        }
                    }
                }

                public void AddInputProperty(string name, bool isStartProperty)
                {
                    GameObject newProperty = GameObject.Instantiate(myController.inputPropertyPrefab);
                    newProperty.transform.SetParent(propertyParent.transform);
                    newProperty.GetComponent<TestControllerProperty>().text.text = name;
                    if (isStartProperty)
                    {
                        startProperties = Array.ConvertAll(ArrayTools.AddElement(newProperty.GetComponent<TestControllerProperty>(), startProperties), item => item as TestControllerProperty);
                    }
                    else
                    {
                        runningProperties = Array.ConvertAll(ArrayTools.AddElement(newProperty.GetComponent<TestControllerProperty>(), runningProperties), item => item as TestControllerProperty);
                    }
                    startButton.transform.SetParent(transform.parent);
                    startButton.transform.SetParent(propertyParent.transform); //Reset button to last position
                }

                public void AddToggleProperty(string name, bool isStartProperty)
                {
                    GameObject newProperty = GameObject.Instantiate(myController.togglePropertyPrefab);
                    newProperty.transform.SetParent(propertyParent.transform);
                    newProperty.GetComponent<TestControllerProperty>().text.text = name;
                    if (isStartProperty)
                    {
                        startProperties = Array.ConvertAll(ArrayTools.AddElement(newProperty.GetComponent<TestControllerProperty>(), startProperties), item => item as TestControllerProperty);
                    }
                    else
                    {
                        runningProperties = Array.ConvertAll(ArrayTools.AddElement(newProperty.GetComponent<TestControllerProperty>(), runningProperties), item => item as TestControllerProperty);
                    }
                    startButton.transform.SetParent(transform.parent);
                    startButton.transform.SetParent(propertyParent.transform); //Reset button to last position
                }

                public void AddButton(string name)
                {
                    GameObject newButton = GameObject.Instantiate(myController.buttonPrefab);
                    newButton.transform.SetParent(buttonParent.transform);
                    newButton.GetComponent<Button>().GetComponentInChildren<Text>().text = name;
                }

                public void ToggleTest(GameObject button)
                {
                    if (!started)
                    {
                        EventReceiver.ClearEvents();
                        values = new Hashtable();
                        foreach (TestControllerProperty p in startProperties)
                        {
                            values.Add(p.text.text, p.GetValue());
                            p.gameObject.SetActive(false);
                        }
                        foreach (TestControllerProperty p in runningProperties)
                        {
                            p.gameObject.SetActive(true);
                        }
                        if (hasTimeLimit)
                        {
                            timeText.transform.parent.gameObject.SetActive(true);
                        }
                        button.GetComponent<Button>().GetComponentInChildren<Text>().text = "Stop Stage";
                        foreach (GameObject g in GameobjectsToActivate)
                        {
                            g.SetActive(true);
                        }
                        for (int i = 0; i < GameobjectsToSendMessageTo.Count; i++)
                        {
                            GameobjectsToSendMessageTo[i].SendMessage(messagesToSend[i]);
                        }
                        foreach (GameObject g in GameobjectsToDeactivate)
                        {
                            g.SetActive(false);
                        }

                        JsonParser.TestStart(values);
                        started = true;
                    }
                    else
                    {
                        foreach (TestControllerProperty p in startProperties)
                        {
                            p.gameObject.SetActive(true);
                            p.Clear();
                        }
                        foreach (TestControllerProperty p in runningProperties)
                        {
                            values.Add(p.text.text, p.GetValue());
                            p.gameObject.SetActive(false);
                        }
                        foreach (GameObject g in GameobjectsToActivate)
                        {
                            g.SetActive(false);
                        }
                        time = 0;
                        timeText.transform.parent.gameObject.SetActive(false);
                        EventReceiver.SendEvents();
                        EventReceiver.ClearEvents();
                        JsonParser.TestEnd();
                        started = false;
                        myController.StageEnded();
                    }

                }

                public static float GetTime()
                {
                    return time;
                }

                public static bool GetStarted()
                {
                    return started;
                }
            }
        }
    }
}
