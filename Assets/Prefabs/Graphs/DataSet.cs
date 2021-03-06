﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataVis.Collaboration
{
    public class DataSet : MonoBehaviour
    {
        public Vector3 MaxValues { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate{ get; set; }

        private List<GameObject> dataPoints;

        public void LoadData(string dataAssetName, int participantIndex, int attributeIndexForY, int attributeIndexForZ, GameObject dataPointPrefab)
        {
            TextAsset data = Resources.Load(dataAssetName) as TextAsset;
            ParticipantDatabase database = JsonUtility.FromJson<ParticipantDatabase>(data.text);
            dataPoints = new List<GameObject>();

            int maxLength = Math.Min(
                database.participants[participantIndex].attributes[attributeIndexForY].attributeData.Count,
                database.participants[participantIndex].attributes[attributeIndexForZ].attributeData.Count);

            DateValuePair pairY, pairZ;
            float maxX = 0, maxY = 0, maxZ = 0;
			DateTime? startDate = null;

            for (int i = 0; i < maxLength; i++)
            {
                maxX++;

                pairY = database.participants[participantIndex].attributes[attributeIndexForY].attributeData[i];
                pairZ = database.participants[participantIndex].attributes[attributeIndexForZ].attributeData[i];

                Vector3 position;
                if (float.TryParse(pairY.value, out position.y) && float.TryParse(pairZ.value, out position.z))
				{
					if (!startDate.HasValue)
					{
						startDate = DateTime.Parse (pairY.date);
//						Debug.Log (participantIndex.ToString () + ": Start Date = " + StartDate.ToString ());
					} 
					EndDate = DateTime.Parse (pairY.date);


                    position.x = i;
                    position.y /= 60;
                    position.z /= 60;

                    maxY = Math.Max(position.y, maxY);
                    maxZ = Math.Max(position.z, maxZ);
                    

                    GameObject newPoint = Instantiate(dataPointPrefab, position, Quaternion.identity, this.transform);
                    newPoint.GetComponent<DataPoint>().id = i;
                    newPoint.GetComponent<DataPoint>().SetLabels("Date: <i>" + DateTime.Parse(pairY.date).ToString("dd/MM/yyyy") + "</i>", "Productivity: <i>" + Math.Round(position.y, 3) + " Hours</i>", "Sleep: <i>" + Math.Round(position.z, 3) + " Hours</i>");
                    newPoint.GetComponent<DataPoint>().values = position;
                    dataPoints.Add(newPoint);
                }
            }
			StartDate = startDate.Value;
//			Debug.Log (participantIndex.ToString () + ": End Date = " + EndDate.ToString ());
//            Debug.Log("x=" + maxX.ToString() + " y=" + maxY.ToString() + " z=" + maxZ.ToString());
            MaxValues = new Vector3(maxX, maxY, maxZ);
        }

        public void ScaleData(float x, float y, float z)
        {
            for (int i = 0; i < dataPoints.Count; i++)
            {
                dataPoints[i].GetComponent<DataPoint>().SetAxisScale(x, y, z);
            }
        }
    }

    [Serializable]
    public class ParticipantDatabase
    {
        public string dbName;
        public List<Participant> participants;
    }

    [Serializable]
    public class Participant
    {
        public string participantName;
        public List<Attribute> attributes;
    }

    [Serializable]
    public class Attribute
    {
        public string attributeName;
        public List<DateValuePair> attributeData;
    }

    [Serializable]
    public class DateValuePair
    {
        public string date;
        public string value;
    }
}

