//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EasyRoads3Dv3;

#if EASYROADS3D_PRO
public class RTC_EasyRoadsEditor : EditorWindow {

    private GUISkin skin;
    private ERRoad[] roads;

    private const int windowWidth = 450;
    private const int windowHeight = 250;

    private float radius = 1.5f;
    private float speed = 80f;
    private float offset = .15f;

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/EasyRoads Integration", false)]
    public static void OpenWindow() {

        GetWindow<RTC_EasyRoadsEditor>(false);

    }

    private void OnEnable() {

        titleContent = new GUIContent("EasyRoads Integration");
        maxSize = new Vector2(windowWidth, windowHeight);
        minSize = maxSize;

        if (!skin)
            skin = Resources.Load("RTC_GUISkin") as GUISkin;

    }

    private void OnGUI() {

        EditorGUILayout.LabelField("This tool will create new lanes and waypoints by selected roads on your scene.");
        EditorGUILayout.LabelField("Simply select road gameobjects on your scene and click generate lanes.");

        if (GUILayout.Button("Select All Roads"))
            SelectAllRoads();

        radius = EditorGUILayout.Slider("Radius", radius, .1f, 5f);
        speed = EditorGUILayout.Slider("Speed", speed, 5f, 240f);
        offset = EditorGUILayout.Slider("Two Ways Offset", offset, .1f, 1f);

        if (GUILayout.Button("Generate Lanes (One Way)"))
            GenerateLanes(1);

        if (GUILayout.Button("Generate Lanes (Two Ways)"))
            GenerateLanes(2);

        if (GUILayout.Button("Update Everything"))
            RTC.UpdateEverything();

        if (GUILayout.Button("Remove All EasyRoads Lanes"))
            RemoveAllEasyRoadsLanes();

    }

    private void SelectAllRoads() {

        List<GameObject> allObjects = new List<GameObject>();
        ERModularRoad[] allRoads = FindObjectsOfType<ERModularRoad>();

        for (int i = 0; i < allRoads.Length; i++)
            allObjects.Add(allRoads[i].gameObject);

        Selection.objects = allObjects.ToArray();

    }

    private void GenerateLanes(int lanes) {

        List<ERModularRoad> roads = new List<ERModularRoad>();

        for (int i = 0; i < Selection.gameObjects.Length; i++) {

            if (Selection.gameObjects[i].GetComponent<ERModularRoad>() != null)
                roads.Add(Selection.gameObjects[i].GetComponent<ERModularRoad>());

        }

        for (int k = 0; k < roads.Count; k++) {

            List<Vector3> indentVecs2 = new List<Vector3>();

            if (lanes == 1) {

                indentVecs2.Clear();

                for (int i = 0; i < roads[k].middleIndentVecs.Count; i++)
                    indentVecs2.Add(roads[k].middleIndentVecs[i]);

                indentVecs2 = ReduceResolution(indentVecs2, 2);

                RTC_Lane newLane = RTC.CreateNewLaneWithWaypoints(indentVecs2.ToArray(), radius, speed);
                IgnoreMiddleWaypointsOnLane(newLane);
                newLane.gameObject.AddComponent<RTC_EasyRoads>();

            }

            if (lanes == 2) {

                indentVecs2.Clear();

                for (int i = 0; i < roads[k].middleIndentVecs.Count; i++)
                    indentVecs2.Add(roads[k].middleIndentVecs[i]);

                for (int i = 0; i < indentVecs2.Count; i++)
                    indentVecs2[i] = Vector3.Lerp(indentVecs2[i], roads[k].leftIndentVecs[i], offset);

                indentVecs2 = ReduceResolution(indentVecs2, 2);

                indentVecs2.RemoveAt(0);

                RTC_Lane newLane = RTC.CreateNewLaneWithWaypoints(indentVecs2.ToArray(), radius, speed);
                IgnoreMiddleWaypointsOnLane(newLane);
                RTC.ReverseCircuit(newLane);
                newLane.gameObject.AddComponent<RTC_EasyRoads>();

                indentVecs2.Clear();

                for (int i = 0; i < roads[k].middleIndentVecs.Count; i++)
                    indentVecs2.Add(roads[k].middleIndentVecs[i]);

                for (int i = 0; i < indentVecs2.Count; i++)
                    indentVecs2[i] = Vector3.Lerp(indentVecs2[i], roads[k].rightIndentVecs[i], offset);

                indentVecs2 = ReduceResolution(indentVecs2, 2);

                indentVecs2.RemoveAt(0);

                RTC_Lane newLane2 = RTC.CreateNewLaneWithWaypoints(indentVecs2.ToArray(), radius, speed);
                IgnoreMiddleWaypointsOnLane(newLane2);
                newLane2.gameObject.AddComponent<RTC_EasyRoads>();

            }

        }

        RTC_EditorCoroutines.Execute(UpdateEverything());

    }

    private IEnumerator UpdateEverything() {

        yield return new WaitForSecondsRealtime(.1f);
        RTC.UpdateEverything();

    }

    private void IgnoreMiddleWaypointsOnLane(RTC_Lane lane) {

        lane.hideMiddleWaypointsGizmos = true;

    }

    private List<Vector3> ReduceResolution(List<Vector3> vectors, int iteration) {

        List<Vector3> vectorsList = new List<Vector3>();

        for (int i = 0; i < vectors.Count; i++)
            vectorsList.Add(vectors[i]);

        for (int i = 0; i < iteration; i++) {

            int a = 0;

            for (int k = 0; k < vectorsList.Count; k++) {

                if (a == 1) {

                    a = 0;

                    if (k != vectorsList.Count - 1)
                        vectorsList.RemoveAt(k);

                }

                a++;

            }

        }

        return vectorsList;

    }

    private void RemoveAllEasyRoadsLanes() {

        RTC_EasyRoads[] allEasyRoads = FindObjectsOfType<RTC_EasyRoads>();

        for (int i = 0; i < allEasyRoads.Length; i++)
            DestroyImmediate(allEasyRoads[i].gameObject);

    }

}
#endif