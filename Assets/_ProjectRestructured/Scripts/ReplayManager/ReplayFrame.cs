using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ProjectRestructured.Scripts.ReplayManager
{
    [Serializable]
    public class ReplayFrame
    {
        public float time;
        public List<ReplayFrameObject> objects;
    }

    [Serializable]
    public class ReplayFrameObject
    {
        public string replayID;
        public ReplayObjectData data;
    }

    [Serializable]
    public class ReplayObjectData
    {
        public Vector3 position;

        public Quaternion rotation;
    }
}