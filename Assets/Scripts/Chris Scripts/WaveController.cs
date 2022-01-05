using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public bool m_IsMoving;

    public float m_WaveHeight = 0.1f;
    public float m_WaveSpeed = 1.0f;
    public float m_WavePeakDistance = 1.0f;
    public float m_WaveAngle = 90.0f;

    public float m_WindWaveCurrentHeight = 0.1f;
    public float m_WindWaveDesiredHeight = 0.1f;
    public float m_WindWaveCurrentSpeed = 0.0f;
    public float m_WindWaveDesiredSpeed = 0.0f;

    public float m_WindStartingSpeed = 0.0f;
    public float m_WindCurrentSpeed = 0.0f;
    private float m_WindDesiredSpeed = 10.0f;

    private float m_WindWaveRatio = 0.0f;
    public float m_WindWavePeakDistance = 1.0f;
    public float m_WindWaveAngle = 90.0f;

    private float m_WindSpeedLerpTime = 5.0f;

    //public float m_WaveANoiseStrength = 1f;
    //public float m_WaveANoiseWalk = 1f;

    public WaterSquare m_GeneratedWaterSquare;
    public float m_PlaneSize;
    public float m_PlaneVertexSpacing;

	WaveDataSet m_PreviousDataSnapshot = new WaveDataSet();
	WaveDataSet m_TransitionWavePreset;
	float m_TransitionDuration;
	float m_TransitionTimeLeft;

    // Use this for initialization
    void Start()
    {
        m_GeneratedWaterSquare = new WaterSquare(gameObject, m_PlaneSize, m_PlaneVertexSpacing);
        m_GeneratedWaterSquare.GenerateMesh();
    }

// Update is called once per frame
void Update ()
    {
		UpdatePresetTransition();
        m_GeneratedWaterSquare.MoveSea(gameObject, Time.timeSinceLevelLoad);

        //if (StaticData.justChangedWindValues)
        //{
        //    StaticData.justChangedWindValues = false;
        //    m_WindWaveRatio = 0.0f;
        //    m_WindStartingSpeed = m_WindCurrentSpeed;
        //    m_WindDesiredSpeed = StaticData.desiredWindSpeed;
        //
        //    m_WindWaveCurrentHeight = Mathf.Clamp((m_WindCurrentSpeed / 15.0f), 0.0f, 1.0f);
        //    m_WindWaveDesiredHeight = Mathf.Clamp((m_WindDesiredSpeed / 15.0f), 0.0f, 1.0f);
        //
        //    m_WindWaveCurrentSpeed = Mathf.Clamp((m_WindCurrentSpeed / 5.0f), 0.1f, 1.0f);
        //    m_WindWaveDesiredSpeed = Mathf.Clamp((m_WindDesiredSpeed / 5.0f), 0.1f, 1.0f);
        //}
        //m_WindWaveRatio += Mathf.Min(Time.deltaTime / 20.0f, 1.0f);

        m_WindWaveCurrentHeight = Mathf.Clamp((StaticData.windSpeed / 15.0f), 0.0f, 1.0f);

        float windAngle = ((Mathf.Atan2(StaticData.windDirection.y, StaticData.windDirection.x) / Mathf.PI) * 180.0f) + 90.0f;
        m_WindWaveAngle = windAngle;
    }

    //Get the y coordinate from whatever wavetype we are using
    public float GetWaveYPos(Vector3 position)
    {
        float normalWaveHeight = WaveTypes.SinXWave(m_WaveAngle, position, m_WaveSpeed, m_WaveHeight, m_WavePeakDistance, 0.0f/*m_WaveANoiseStrength*/, 0.0f/*m_WaveANoiseWalk*/, Time.timeSinceLevelLoad);
        float windWaveHeight = WaveTypes.SinXWave(m_WindWaveAngle, position, m_WaveSpeed, m_WindWaveCurrentHeight, m_WindWavePeakDistance, 0.0f/*m_WaveANoiseStrength*/, 0.0f/*m_WaveANoiseWalk*/, Time.timeSinceLevelLoad);
        //float desiredWindWaveHeight = WaveTypes.SinXWave(m_WindWaveAngle, position, m_WindWaveDesiredSpeed, m_WindWaveDesiredHeight, m_WindWavePeakDistance, 0.0f/*m_WaveANoiseStrength*/, 0.0f/*m_WaveANoiseWalk*/, Time.timeSinceLevelLoad);
        //windWaveHeight = Mathf.Lerp(windWaveHeight, desiredWindWaveHeight, m_WindWaveRatio);

        return normalWaveHeight + windWaveHeight;
    }

    //Find the distance from a vertice to water
    //Make sure the position is in global coordinates
    //Positive if above water
    //Negative if below water
    public float DistanceToWater(Vector3 position)
    {
        float waterHeight = GetWaveYPos(position);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }

	public void TransitionToPreset(WaveDataSet _preset, float _transitionDuration)
	{
		SnapshotPreviousData();
		m_TransitionWavePreset = _preset;
		m_TransitionDuration = _transitionDuration;
		m_TransitionTimeLeft = _transitionDuration;
	}

	void SnapshotPreviousData()
	{
		m_PreviousDataSnapshot.m_WaveHeight = m_WaveHeight;
		//m_PreviousDataSnapshot.m_WaveSpeed = m_WaveSpeed;
		m_PreviousDataSnapshot.m_WavePeakDistance = m_WavePeakDistance;
		m_PreviousDataSnapshot.m_WaveAngle = m_WaveAngle;
	}

	void UpdatePresetTransition()
	{
		if (m_TransitionTimeLeft > 0.0f)
		{
			m_TransitionTimeLeft -= Time.deltaTime;
			if (m_TransitionTimeLeft < 0.0f)
				m_TransitionTimeLeft = 0.0f;
			
			ApplyTransitionLerp();

			if (m_TransitionTimeLeft <= 0.0f)
			{
				m_TransitionWavePreset = null;
			}
		}
	}

	void ApplyTransitionLerp()
	{
		float t = (m_TransitionDuration - m_TransitionTimeLeft) / m_TransitionDuration;

		m_WaveHeight = Mathf.Lerp(m_PreviousDataSnapshot.m_WaveHeight, m_TransitionWavePreset.m_WaveHeight, t);
		//m_WaveSpeed = Mathf.Lerp(m_PreviousDataSnapshot.m_WaveSpeed, m_TransitionWavePreset.m_WaveSpeed, t);
		m_WavePeakDistance = Mathf.Lerp(m_PreviousDataSnapshot.m_WavePeakDistance, m_TransitionWavePreset.m_WavePeakDistance, t);
		m_WaveAngle = Mathf.Lerp(m_PreviousDataSnapshot.m_WaveAngle, m_TransitionWavePreset.m_WaveAngle, t);
	}
}

public class WaveTypes
{
    //Sinus waves
    public static float SinXWave(
        float waveAngle,
        Vector3 position,
        float speed,
        float scale,
        float waveDistance,
        float noiseStrength,
        float noiseWalk,
        float timeSinceStart)
    {
        Quaternion rotation = Quaternion.Euler(0, waveAngle, 0.0f);
        Vector3 rotatedVector = rotation * position;

        float x = rotatedVector.x;
        float y = 0.0f;
        float z = rotatedVector.z;

        //Using only x or z will produce straight waves
        //Using only y will produce an up/down movement
        //x + y + z rolling waves
        //x * z produces a moving sea without rolling waves

        float waveType = z;

        y += Mathf.Sin((timeSinceStart * speed + waveType) / waveDistance) * scale;



        //Add noise to make it more realistic
        //y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
    }
}

//Generates a plane with a specific resolution and transforms the plane to make waves
public class WaterSquare
{
    public Transform squareTransform;

    //Add the wave mesh to the MeshFilter
    public MeshFilter terrainMeshFilter;

    //The total size in m
    private float size;
    //Resolution = Width of one square
    public float spacing;
    //The total number of vertices we need to generate based on size and spacing
    private int width;

    //For the thread to update the water
    //The local center position of this square to fake transformpoint in a thread
    //public Vector3 centerPos;
    //The latest vertices that belong to this square
    public Vector3[] vertices;

    public WaterSquare(GameObject waterSquareObj, float size, float spacing)
    {
        this.squareTransform = waterSquareObj.transform;

        this.size = size;
        this.spacing = spacing;

        this.terrainMeshFilter = squareTransform.GetComponent<MeshFilter>();


        //Calculate the data we need to generate the water mesh   
        width = (int)(size / spacing);
        //Because each square is 2 vertices, so we need one more
        width += 1;

        //Center the sea
        float offset = -((width - 1) * spacing) / 2;

        Vector3 newPos = new Vector3(offset, -squareTransform.position.y * 0.5f, offset);

        squareTransform.position += newPos;

        //Save the center position of the square
        //this.centerPos = waterSquareObj.transform.localPosition;


        //Generate the sea
        //To calculate the time it took to generate the terrain
        float startTime = System.Environment.TickCount;

        GenerateMesh();

        //Calculate the time it took to generate the terrain in seconds
        float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f;

        Debug.Log("Sea was generated in " + timeToGenerateSea.ToString() + " seconds");


        //Save the vertices so we can update them in a thread
        this.vertices = terrainMeshFilter.mesh.vertices;
    }


    //Generate the water mesh
    public void GenerateMesh()
    {
        //Vertices
        List<Vector3[]> verts = new List<Vector3[]>();
        //Triangles
        List<int> tris = new List<int>();
        //Texturing
        //List<Vector2> uvs = new List<Vector2>();

        for (int z = 0; z < width; z++)
        {

            verts.Add(new Vector3[width]);

            for (int x = 0; x < width; x++)
            {
                Vector3 current_point = new Vector3();

                //Get the corrdinates of the vertice
                current_point.x = x * spacing;
                current_point.z = z * spacing;
                current_point.y = squareTransform.position.y;

                verts[z][x] = current_point;

                //uvs.Add(new Vector2(x,z));

                //Don't generate a triangle the first coordinate on each row
                //Because that's just one point
                if (x <= 0 || z <= 0)
                {
                    continue;
                }

                //Each square consists of 2 triangles

                //The triangle south-west of the vertice
                tris.Add(x + z * width);
                tris.Add(x + (z - 1) * width);
                tris.Add((x - 1) + (z - 1) * width);

                //The triangle west-south of the vertice
                tris.Add(x + z * width);
                tris.Add((x - 1) + (z - 1) * width);
                tris.Add((x - 1) + z * width);
            }
        }

        //Unfold the 2d array of verticies into a 1d array.
        Vector3[] unfolded_verts = new Vector3[width * width];

        int i = 0;
        foreach (Vector3[] v in verts)
        {
            //Copies all the elements of the current 1D-array to the specified 1D-array
            v.CopyTo(unfolded_verts, i * width);

            i++;
        }

        //Generate the mesh object
        Mesh newMesh = new Mesh();
        newMesh.vertices = unfolded_verts;
        //newMesh.uv = uvs.ToArray();
        newMesh.triangles = tris.ToArray();

        //Ensure the bounding volume is correct
        newMesh.RecalculateBounds();
        //Update the normals to reflect the change
        newMesh.RecalculateNormals();


        //Add the generated mesh to this GameObject
        terrainMeshFilter.mesh.Clear();
        terrainMeshFilter.mesh = newMesh;
        terrainMeshFilter.mesh.name = "Water Mesh";

        Debug.Log(terrainMeshFilter.mesh.vertices.Length);
    }

    public void MoveSea(GameObject oceanObject, float timeSinceStart)
    {
        Vector3[] vertices = terrainMeshFilter.mesh.vertices;
        WaveController waterController = oceanObject.GetComponent<WaveController>();
    
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
    
            //From local to global
            //Vector3 vertexGlobal = squareTransform.TransformPoint(vertex);
    
            Vector3 vertexGlobal = vertex + oceanObject.transform.position;
    
            //Unnecessary because no rotation nor scale
            //Vector3 vertexGlobalTest2 = squareTransform.rotation * Vector3.Scale(vertex, squareTransform.localScale) + squareTransform.position;
    
            //Debug 
            if (i == 0)
            {
                //Debug.Log(vertexGlobal + " " + vertexGlobalTest);
            }
    
            //Get the water height at this coordinate
            vertex.y = waterController.GetWaveYPos(vertexGlobal);
    
            //From global to local - not needed if we use the saved local x,z position
            //vertices[i] = transform.InverseTransformPoint(vertex);
    
            //Don't need to go from global to local because the y pos is always at 0
            vertices[i] = vertex;
        }
    
        terrainMeshFilter.mesh.vertices = vertices;
    
        terrainMeshFilter.mesh.RecalculateNormals();
    }
}
