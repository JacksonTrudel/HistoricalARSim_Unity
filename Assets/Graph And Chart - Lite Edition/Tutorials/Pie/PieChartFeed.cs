using UnityEngine;
using System.Collections;
using ChartAndGraph;
public class PieChartFeed : MonoBehaviour
{
	void Start ()
    {
        PieChart pie = GetComponent<PieChart>();
        if (pie != null)
        {
            pie.DataSource.SlideValue("test1", 50, 10f);
            pie.DataSource.SetValue("test2", Random.value * 10);
        }
	}
}
