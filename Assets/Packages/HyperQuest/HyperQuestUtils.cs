using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;


public static class Utils
{
    public static void AddDecayForce(this Rigidbody2D rb, Vector2 force, float decayTime = 0.25f)
    {
        PlayerManager.I.StartCoroutine(DecayForce_Co());
        IEnumerator DecayForce_Co()
        {
            float decaySpeed = 1f / decayTime;
            while(force.magnitude > 0)
            {
                if (rb == null) yield break;
                Debug.DrawLine(rb.transform.position, rb.transform.position + (Vector3)force, Color.blue.Lerp(Color.red, 0.5f));
                var decayForce = -force.normalized * decaySpeed * Time.fixedDeltaTime;
                if (force.magnitude < 0.001f || decayForce.magnitude > force.magnitude)
                {
                    force = Vector2.zero;
                }
                else
                {
                    force += decayForce;
                }
                rb.AddDampForce(10f * force, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    #region Coroutines
    public static void DelayedInvoke(this MonoBehaviour mb, float delayTime, Action function, bool useTimescale = false)
    {
        mb.StartCoroutine(DelayedInvoke_Coroutine(mb, delayTime, function, useTimescale));
    }
    static IEnumerator DelayedInvoke_Coroutine(MonoBehaviour monoBehavior, float delayTime, Action function, bool useTimescale = false)
    {
        if (delayTime <= 0f)
        {
            for (; delayTime < 0f; delayTime++)
            {
                yield return null;
            }
        }
        else if(!useTimescale)
            yield return new WaitForSecondsRealtime(delayTime);
        else
        {
            while(delayTime > 0f)
            {
                delayTime -= Time.deltaTime;
                yield return null;
            }
        }

        if (monoBehavior != null)
        {
            function?.Invoke();
        }
    }
    #endregion

    #region Mouse
    static public Vector2 GetMouseWorldPos()
    {
        Vector2 pos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    internal static void RandomSeed()
    {
        Random.InitState(DateTime.Now.Ticks.GetHashCode());
    }

    static public Collider2D[] MouseOverlap2D(float radius = 0.1f)
    {
        return Physics2D.OverlapCircleAll(GetMouseWorldPos(), radius);
    }
    #endregion

    /// <summary>
    /// Spring-damped angle smoothing — behaves like SmoothDampAngle but 
    /// allows overshoot by using an underdamped spring.
    /// frequency: Hz (stiffness)
    /// damping:   0 = full overshoot, 1 = critically damped, >1 hard damping
    /// </summary>
    public static float SpringDampAngle(
    float currentAngle,
    float targetAngle,
    ref float angularVelocity,
    float frequency = 2f,
    float damping = 0.3f,
    float maxSpeed = Mathf.Infinity,  // deg/sec
    float maxAccel = Mathf.Infinity)  // deg/sec^2
    {
        float dt = Time.deltaTime;
        if (dt <= 0f)
            return currentAngle;

        // Signed shortest delta [-180, 180]
        float delta = Mathf.DeltaAngle(currentAngle, targetAngle);

        // Continuous spring parameters
        float omega = 2f * Mathf.PI * frequency;

        // θ'' = ω² * delta − 2ζω * θ'
        float accel = omega * omega * delta - 2f * damping * omega * angularVelocity;

        // Clamp acceleration so we *ease into* motion instead of instantly spiking
        if (!float.IsInfinity(maxAccel))
            accel = Mathf.Clamp(accel, -maxAccel, maxAccel);

        // Integrate velocity
        angularVelocity += accel * dt;

        // Clamp velocity (deg/sec), keeps it from snapping too fast
        if (!float.IsInfinity(maxSpeed))
            angularVelocity = Mathf.Clamp(angularVelocity, -maxSpeed, maxSpeed);

        // Integrate angle
        currentAngle += angularVelocity * dt;

        return currentAngle;
    }

    /// <summary>
    /// Inertial motor aim:
    /// - torque:        max angular accel at full error (deg/sec²)
    /// - responseAngle: error (deg) where torque reaches full strength
    /// - drag:          Unity-style linear drag (per second)
    ///
    /// angularVel is deg/sec and updated by ref.
    /// </summary>
    public static float MotorDampAngle(
        float currentAngle,
        float targetAngle,
        ref float angularVel,
        float torque,
        float responseAngle,
        float drag,
        float deltaTime)
    {
        if (deltaTime <= 0f)
            return currentAngle;

        // Signed shortest difference [-180, 180]
        float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

        // --- Motor falloff: linear near zero, saturates at +/-1 ---
        float deltaAngleNorm = deltaAngle / responseAngle;
        deltaAngleNorm = Mathf.Clamp(deltaAngleNorm, -1f, 1f);

        // Motor torque toward target
        float motorAccel = torque * deltaAngleNorm;

        // Integrate velocity using motor force
        angularVel += motorAccel * deltaTime;

        // --- Unity-style drag (linear damping) ---
        // SAME formula Unity uses for Rigidbody.drag:
        // v *= 1 - drag * dt   (clamped so it never flips)
        float dragFactor = 1f - drag * deltaTime;
        if (dragFactor < 0f) dragFactor = 0f;
        angularVel *= dragFactor;

        // Integrate angle from velocity
        currentAngle += angularVel * deltaTime;

        return currentAngle;
    }

    public static float GeneratePerlinOctaveNoise(float x, float y, int octaves, Vector2 noiseOffset, float noiseScale, float persistence, float lacunarity)
    {
        float total = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            // Calculate Perlin noise at this octave
            float sampleX = (x + noiseOffset.x) / noiseScale * frequency;
            float sampleY = (y + noiseOffset.y) / noiseScale * frequency;
            float perlin = Mathf.PerlinNoise(sampleX, sampleY);

            total += perlin * amplitude;
            maxValue += amplitude;

            amplitude *= persistence; // Reduce amplitude
            frequency *= lacunarity;  // Increase frequency
        }

        return total / maxValue; // Normalize result to 0-1
    }

    public static Vector2 PerlinV2(float x, float y, float scale = 1f, int offset = 0)
    {
        Vector2 perlinVector = new Vector2();
        perlinVector.x = Mathf.PerlinNoise(y * scale + offset, x * scale + offset)*2f-1f;
        perlinVector.y = Mathf.PerlinNoise(x * scale + offset, y * scale + offset)*2f-1f;
        return perlinVector;
    }
    public static Vector2 PerlinV2(float seed, float scale = 1f, int offset = 100)
    {
        return PerlinV2(seed, seed, scale, offset);
    }
    public static Vector2 GenerateRandomVector2()
    {
        float angle = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, 0f, angle) * Vector2.up;
    }

    public static string SecondsToMonoTimeString(this float time, float fontSize)
    {
        // Calculate minutes, seconds, and milliseconds
        int minutes = Mathf.FloorToInt(time / 60);              // Get the total minutes
        int seconds = Mathf.FloorToInt(time % 60);              // Get the seconds
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000 / 10f); // Get two-digit milliseconds


        // Convert the numeric parts to monospaced using your Monospace extension
        string spacing = (fontSize / 2f).ToString("F0") + "px";
        string minMonospaced = minutes.ToString("00").Monospace(spacing);
        string secMonospaced = seconds.ToString("00").Monospace(spacing);
        string msMonospaced = milliseconds.ToString("00").Monospace(spacing);

        // Combine everything with normal separators
        return minMonospaced + ":" + secMonospaced + "." + msMonospaced;
    }
    public static string CapitalizeFirstLetters(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, @"\b[a-z]", m => m.Value.ToUpper());
    }

    public static void SetFullTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * timeScale;
    }

    public static void SetDefaultTimeScale(float timeScale)
    {
        PlayerPrefs.SetFloat("default_timescale", timeScale);
        Utils.SetFullTimeScale(timeScale);
    }

    #region Physics
    public static List<T> ComponentScan<T>(Vector2 point, float radius, bool sortByDist = true, LayerMask? layerMask = null) where T : MonoBehaviour
    {
        // Scan & Filter for components
        List<T> list = new List<T>();
        Collider2D[] results = layerMask == null ? Physics2D.OverlapCircleAll(point, radius) : Physics2D.OverlapCircleAll(point, radius, layerMask.Value);
        new List<Collider2D>(results).ForEach(col =>
        {
            T component = col.GetComponentInParent<T>();
            if (component != null) { list.Add(component); }
        });

        // Sort by distance
        if (sortByDist) list = list.GetCopySortedByDistance(point);

        return list;
    }
    public static List<T> ComponentCircleCastAll<T>(Vector2 origin, float radius, Vector2 direction, float distance) where T : MonoBehaviour
    {
        List<T> list = new List<T>();
        RaycastHit2D[] hitArray = Physics2D.CircleCastAll(origin, radius, direction, distance);
        foreach(RaycastHit2D hit in hitArray)
        {
            T comp = hit.collider.GetComponent<T>();
            if (comp != null)
            {
                list.Add(comp);
            }
        }
        return list;
    }

    public static List<RaycastHit2D> RadialCast2D(Vector2 origin, float distance, int rayCount, LayerMask? layerMask = null, Vector2? startDir = null, float range = 360f) => RadialCast2D(origin, distance, rayCount, layerMask, startDir == null? 0f : Vector2.SignedAngle(Vector2.up, startDir.Value), range);
    public static List<RaycastHit2D> RadialCast2D(Vector2 origin, float distance, int rayCount, LayerMask? layerMask = null, float startAngle = 0f, float range = 360f)
    {
        bool queriesStartInColliders = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        float deltaAngle = range / rayCount;
        for(int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + deltaAngle * i - range * 0.5f;
            Vector2 castDirection = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
            RaycastHit2D hit = layerMask == null? Physics2D.Raycast(origin, castDirection, distance) : Physics2D.Raycast(origin, castDirection, distance, layerMask.Value);
            if(hit)
            {
                hitList.Add(hit);
            }
        }

        Physics2D.queriesStartInColliders = queriesStartInColliders;
        return hitList;
    }

    #endregion
}

public static class TypeExtensions
{

    #region Type Extensions

    #region Float Extensions
    /// <summary>
    /// Remaps a value from a to b onto x to y.
    /// </summary>
    /// <param name="value">Value to map.</param>
    /// <param name="a">Initial starting value.</param>
    /// <param name="b">Initial ending value.</param>
    /// <param name="x">New starting value.</param>
    /// <param name="y">New ending vaue.</param>
    /// <param name="doClamp">Whether or not to clamp the mapped value between x and y.</param>
    /// <returns></returns>
    public static float Remap(this int value, float a, float b, float x, float y, bool doClamp = true) => ((float)value).Remap(a, b, x, y, doClamp);
    public static float Remap(this float value, float a, float b, float x, float y, bool doClamp = true)
    {
        if (a == b) return x;

        float result = (value - a) / (b - a) * (y - x) + x;
        if (doClamp)
        {
            result = Mathf.Clamp(result, Mathf.Min(x, y), Mathf.Max(x, y));
        }
        return result;
    }
    public static float RemapPercent(this float percent, float x, float y, bool doClamp = true)
    {
        return Remap(percent, 0f, 1f, x, y, doClamp);
    }
    public static float Clamp(this float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    public static float Clamp01(this float value)
    {
        return Mathf.Clamp01(value);
    }

    public static int ClampMin(this int value, int min)
    {
        return value < min ? min : value;
    }
    public static float ClampMin(this float value, float min)
    {
        return value < min ? min : value;
    }
    public static int ClampInt(this int value, int min, int max)
    {
        return value > max ? max : (value < min ? min : value);
    }
    public static int ClampMax(this int value, int max)
    {
        return value > max ? max : value;
    }
    public static float ClampMax(this float value, float max)
    {
        return value > max ? max : value;
    }
    /// <summary>
    /// Adds a randomly positive or negative value from 0 to magnitude.
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="magnitude"></param>
    /// <returns></returns>
    public static float Randomize(this float baseValue, float magnitude)
    {
        return baseValue + Random.Range(-1f, 1f) * magnitude;
    }
    public static float Pow(this float value, float power)
    {
        return Mathf.Pow(value, power);
    }

    public static float Abs(this float value) => Mathf.Abs(value);
    public static float Floor(this float value) => Mathf.Floor(value);
    public static float Sign(this float value) => Mathf.Sign(value);
    public static float Lerp(this float value, float target, float lerpAmount)
    {
        return Mathf.Lerp(value, target, lerpAmount);
    }
    public static float LerpAngle(this float angle, float target, float lerpAmount)
    {
        return Mathf.LerpAngle(angle, target, lerpAmount);
    }
    public static float SnapAngle(this float angle, float rotationSnap)
    {
        if (rotationSnap == 0) return angle;
        int snapCount = Mathf.RoundToInt(angle / rotationSnap);
        return snapCount * rotationSnap;
    }
    public static float TruncateToDecimalPlaces(this float value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return Mathf.Floor(value * multiplier) / multiplier;
    }
    #endregion

    #region Int Extensions
    public static bool IsLayerInLayermask(this int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
    #endregion

    #region string Extensions
    public static int ParseInt(this string s)
    {
        Int32.TryParse(s, out int result);
        return result;
    }
    #endregion

    #region List Extensions
    public static T Last<T>(this List<T> list, int index = 0)
    {
        if (list.Count - 1 - index < 0) return default(T);
        return list[list.Count - 1 - index];
    }

    public static List<T> GetLastRange<T>(this List<T> list, int count, int index=0)
    {
        if (count == 0) return new List<T>();
        index = list.Count - count - index;

        if (index < 0) index = 0;
        if (index + count >= list.Count) count = list.Count - index;

        return list.GetRange(index, count);
    }

    public static List<T> GetMidRange<T>(this List<T> list, int midIndex, int range)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }
        if (range < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(range), "Range cannot be negative.");
        }
        if (midIndex < 0 || midIndex >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(midIndex), "MidIndex must be within the bounds of the list.");
        }

        // Calculate start index; ensure it does not go below 0
        int startIndex = Math.Max(midIndex - range, 0);

        // Calculate end index; ensure it does not exceed the list's count
        int endIndex = Math.Min(midIndex + range, list.Count - 1);

        // Calculate count of elements to include
        int count = endIndex - startIndex + 1;

        return list.GetRange(startIndex, count);
    }

    public static List<T> GetCopySortedByDistance<T>(this List<T> list, Vector2 origin) where T : MonoBehaviour
    {
        T[] array = list.ToArray();
        Array.Sort(array, (x, y) =>
        {
            return (int)Mathf.Sign(Vector2.Distance(x.transform.position, origin) - Vector2.Distance(y.transform.position, origin));
        });
        list.Clear();
        list.AddRange(array);
        return list;
    }
    public static T GetRandomElement<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        return list[Random.Range(0, list.Count)];
    }
    public static void Shuffle<T>(this List<T> list)
    {
        int swapCount = list.Count * 2;
        for (int i = 0; i < swapCount; i++)
        {
            int indexA = Random.Range(0, list.Count);
            int indexB = Random.Range(0, list.Count);
            if (indexA == indexB) continue;

            // Swap
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
    public static bool GetKey(this List<KeyCode> keyList)
    {
        foreach(KeyCode c in keyList)
        {
            if (Input.GetKey(c)) return true;
        }
        return false;
    }
    public static bool GetKeyDown(this List<KeyCode> keyList)
    {
        foreach(KeyCode c in keyList)
        {
            if (Input.GetKeyDown(c)) return true;
        }
        return false;
    }
    
    public static Vector2 Direciton(this List<Vector2> list, int index = 0)
    {
        if (list.Count < 2) return Vector2.zero;
        if(index == 0) return list[1] - list[0];
        return list[index] - list[index - 1];
    }
    public static Vector2 LastDirection(this List<Vector2> list, int index = 0)
    {
        if (list.Count < 2) return Vector2.zero;
        if (index == 0) return list[list.Count - 1] - list[list.Count - 2];
        return list[list.Count - 1 - index] - list[list.Count - 1 - index - 1];
    }
    public static List<Vector2> AddRangeReturn(this List<Vector2> list, List<Vector2> otherList)
    {
        List<Vector2> newList = new List<Vector2>();
        newList.AddRange(list);
        newList.AddRange(otherList);
        return newList;
    }
    public static float GetDistance(this List<Vector2> list)
    {
        float dist = 0;
        for(int i = 0; i < list.Count-1; i++)
        {
            dist += Vector2.Distance(list[i], list[i + 1]);
        }
        return dist;
    }
    #endregion

    #region Vector Extensions
    public static Vector2 ClampMagnitude(this Vector2 vector, float magnitude)
    {
        return Vector2.ClampMagnitude(vector, magnitude);
    }
    public static Vector3 VectorTowards(this Vector3 startVector, Vector3 endVector)
    {
        return endVector - startVector;
    }
    public static Vector2 AngleToVector(this float angle)
    {
        return Vector2.up.Rotate2D(angle);
    }

    public static Vector3 AverageVectors(this List<Vector3> list)
    {
        Vector3 avg = new Vector3();
        list.ForEach(v => avg += v);
        avg /= list.Count;
        return avg;
    }
    public static Vector2 AverageVectors(this List<Vector2> list)
    {
        Vector2 avg = new Vector2();
        list.ForEach(v => avg += v);
        avg /= list.Count;
        return avg;
    }
    public static Vector2 SumVectors(this List<Vector2> list)
    {
        Vector2 v = new Vector2();
        list.ForEach(x => v += x);
        return v;
    }
    public static Vector2Int ToVector2Int(this Vector2 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }
    public static float GetSignedAngleTo(this Vector2 thisVector, Vector2 otherVector)
    {
        return Vector2.SignedAngle(otherVector, thisVector);
    }
    public static Vector2 RotateVector(this Vector3 thisVector, float angle)
    {
        return Quaternion.Euler(0f, 0f, angle) * thisVector;
    }
    public static Vector3 Lerp(this Vector3 thisVector, Vector3 otherVector, float lerp)
    {
        return Vector3.Lerp(thisVector, otherVector, lerp);
    }
    public static Vector2 Lerp(this Vector2 thisVector, Vector2 otherVector, float lerp)
    {
        return Vector2.Lerp(thisVector, otherVector, lerp);
    }
    public static Vector2 ClampMagnitude(this Vector2 thisVector, float minMagnitude, float maxMagnitude)
    {
        float mag = thisVector.magnitude;
        if (mag < minMagnitude) return thisVector.normalized * minMagnitude;
        if (mag > maxMagnitude) return thisVector.normalized * maxMagnitude;
        return thisVector;
    }
    public static Vector2 TruncateVector2(this Vector2 point, int decimalPlaces = 2)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        float truncatedX = Mathf.Floor(point.x * multiplier) / multiplier;
        float truncatedY = Mathf.Floor(point.y * multiplier) / multiplier;
        return new Vector2(truncatedX, truncatedY);
    }
    public static Vector3 TruncateVector3(this Vector3 point, int decimalPlaces = 2)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        float truncatedX = Mathf.Floor(point.x * multiplier) / multiplier;
        float truncatedY = Mathf.Floor(point.y * multiplier) / multiplier;
        float truncatedZ = MathF.Floor(point.z * multiplier) / multiplier;
        return new Vector3(truncatedX, truncatedY, truncatedZ);
    }
    public static Vector2 RotationalLerp(this Vector2 thisVector, Vector2 otherVector, float lerp)
    {
        float angle = Vector2.SignedAngle(thisVector, otherVector);
        angle *= lerp;
        return Quaternion.Euler(0f, 0f, angle) * thisVector;
    }
    public static Vector2 Abs(this Vector2 thisVector)
    {
        return new Vector2(thisVector.x.Abs(), thisVector.y.Abs());
    }
    public static Vector3 Abs(this Vector3 thisVector)
    {
        return new Vector3(thisVector.x.Abs(), thisVector.y.Abs(), thisVector.z.Abs());
    }
    public static Vector2 Rotate2D(this Vector2 thisVector, float angle)
    {
        return Quaternion.Euler(0f, 0f, angle) * thisVector;
    }
    public static Vector2 DirectionTo(this Vector2 pointA, Vector2 pointB)
    {
        return (pointB - pointA).normalized;
    }

    public static float GetClockwiseAngle(this Vector2 v1, Vector2 v2)
    {
        // Normalize vectors
        v1.Normalize();
        v2.Normalize();

        // Calculate the dot and cross product
        float dot = Vector3.Dot(v1, v2);
        float det = v1.x * v2.y - v1.y * v2.x;

        // Calculate the angle in radians
        float angle = Mathf.Atan2(det, dot);

        // Convert radians to degrees
        angle *= Mathf.Rad2Deg;

        // Adjust the angle for clockwise calculation
        if (angle < 0)
        {
            angle = -angle; // If angle is negative, it's already the clockwise angle
        }
        else
        {
            angle = 360 - angle; // Otherwise, subtract from 360 for the clockwise angle
        }

        return angle;
    }
    #endregion

    #region Color Extensions
    public static Color Alpha(this Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
    public static Color Lerp(this Color c, Color otherColor, float amount)
    {
        return Color.Lerp(c, otherColor, amount);
    }
    public static Color AdjustHSV(this Color c, float offsetH, float offsetS, float offsetV)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(h + offsetH, s + offsetS, v + offsetV);
    }
    public static Color SetHue(this Color c, float newHue)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(newHue, s, v);
    }

    public static Color SetHS(this Color c, float newHue, float newSaturation)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(newHue, newSaturation, v);
    }

    public static Color SetSaturation(this Color c, float newSaturation)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(h, newSaturation, v);
    }

    public static Color SetValue(this Color c, float newValue)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(h, s, newValue);
    }
    public static float GetHue(this Color c)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return h;
    }

    public static float GetValue(this Color c)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return v;
    }

    public static float GetSaturation(this Color c)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return s;
    }
    #endregion

    #region Transform Extensions
    public static float GetAverageSize(this Transform t)
    {
        return (t.localScale.x + t.localScale.y + t.localScale.z) / 3f;
    }

    public static float DistanceFrom(this Transform t, Transform other)
    {
        return Vector3.Distance(t.position, other.position);
    }
    public static Vector3 VectorTowards(this Transform t, Vector3 pos)
    {
        return pos - t.position;
    }
    public static Vector3 VectorTowards(this Transform t, Transform other)
    {
        return t.VectorTowards(other.position);
    }
    public static Vector2 VectorTowards2D(this Transform t, Vector2 pos)
    {
        return pos - (Vector2)t.position;
    }
    public static Vector2 VectorTowards2D(this Transform t, Transform other)
    {
        return t.VectorTowards2D(other.transform.position);
    }

    public static void SetLossyScale(this Transform t, Vector3 scale)
    {
        Transform parent = t.parent;
        t.SetParent(null);
        t.localScale = scale;
        t.SetParent(parent);
    }

    public static void Rotate2D(this Transform t, float angle)
    {
        t.rotation = Quaternion.Euler(t.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z + angle);
    }
    #endregion

    #region GameObject Extensions
    public static void SetCollidersEnabled2D(this GameObject go, bool enabled, bool includeInactive = false)
    {
        foreach (Collider2D col in go.GetComponentsInChildren<Collider2D>(includeInactive))
        {
            col.enabled = enabled;
        }
    }
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T comp = go.GetComponent<T>();
        if (comp != null) return comp;
        return go.AddComponent<T>();
    }
    public static bool IsInLayermask(this GameObject go, LayerMask layermask)
    {
        return go.layer.IsLayerInLayermask(layermask);
    }
    #endregion

    #region RigidBody2D Extensions
    public static void AddDampForce(this Rigidbody2D rb, Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        rb.AddForce(force * rb.linearDamping, forceMode);
    }
    #endregion

    #region IEnumerator Extensions
    public static T Rand<T>(this IEnumerable<T> source)
    {
        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements");

        return list[Random.Range(0, list.Count)];
    }
    public static T Rand<T>(this IEnumerable<T> source, int seed)
    {
        var list = source as IList<T> ?? source.ToList();

        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements");

        Random.InitState(seed);
        return list[Random.Range(0, list.Count)];
    }
    public static Coroutine StartCoroutine(this IEnumerator iEnum, MonoBehaviour mono)
    {
        return mono.StartCoroutine(iEnum);
    }
    #endregion

    #endregion

    public static Vector2 GetMirroredSamplePoint(this Texture2D texture, Vector2 samplePoint)
    {
        samplePoint.x += texture.width * 0.5f;
        samplePoint.y += texture.height * 0.5f;

        // Ensure the sample point is in the range of the texture's width and height
        int x = (int)(samplePoint.x.Abs() % (texture.width * 2));
        int y = (int)(samplePoint.y.Abs() % (texture.height * 2));

        // If the sample point is beyond the texture's width/height, mirror the coordinates
        if (x >= texture.width) x = texture.width * 2 - x - 1;
        if (y >= texture.height) y = texture.height * 2 - y - 1;

        return new Vector2(x, y);
    }
}

/// <summary>
/// Extension class for easily adding rich text tags to strings.
/// </summary>
public static class RichTextExtensions
{
    public static string Bold(this string s)
    {
        return "<b>" + s + "</b>";
    }

    public static string Strikethrough(this string s)
    {
        return $"<s>{s}</s>";
    }

    public static string Italic(this string s)
    {
        return "<i>" + s + "</i>";
    }

    public static string Size(this string s, int pixelSize)
    {
        return "<size=" + pixelSize + ">" + s + "</size>";
    }

    public static string Color(this string s, string hexColor)
    {
        return "<color=#" + hexColor + ">" + s + "</color>";
    }

    public static string Color(this string s, Color c)
    {
        string hexColor = ColorUtility.ToHtmlStringRGBA(c);
        return s.Color(hexColor);
    }

    public static string Monospace(this string s, string spacing="55px")
    {
        return $"<mspace={spacing}>{s}</mspace>";
    }
    public static string Monospace(this string s, float spacing) => s.Monospace(spacing.ToString("F0") + "px");

    public static string Material(this string s, int matIndex)
    {
        return "<material=" + matIndex + ">" + s + "</material>";
    }

    public static string Align(this string s, string alignmentString)
    {
        return $"<align={alignmentString}>" + s;
    }

    public static string LineHeight(this string s, string height)
    {
        return $"<line-height={height}>" + s;
    }

    public static string LeftRightString(string leftString, string rightString)
    {
        return $"<align=left>{leftString}<line-height=0>\n<align=right>{rightString}<line-height=1em>";
    }
}

public static class GizmoExtensions
{
    public static void DrawSemicircle(Vector2 position, Vector2 direction, float radius, float angle)
    {
        int segments = 20; // Adjust for more or less granularity
        float angleStep = angle / segments;
        direction.Normalize();
        Quaternion initialRotation = Quaternion.FromToRotation(Vector2.right, direction);
        Quaternion rotationStep = Quaternion.Euler(0, 0, -angle / 2);

        Vector2 previousPoint = position + (Vector2)(initialRotation * rotationStep * Vector2.right * radius);

        // Draw line from center to start of semicircle
        Gizmos.DrawLine(position, previousPoint);
        Gizmos.DrawLine(position, position + direction * radius);

        for (int i = 1; i <= segments; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, angleStep * i);
            Vector2 directionStep = initialRotation * rotationStep * rot * Vector2.right;
            Vector2 currentPoint = position + directionStep * radius;

            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;

            if (i == segments)
            {
                // Draw line from center to end of semicircle
                Gizmos.DrawLine(position, currentPoint);
            }
        }
    }

    public static void DrawPath(List<Vector2> pathList, Color? c, float nodeDiameter = 0f)
    {
        Gizmos.color = c?? Color.white;
        for(int i = 0; i < pathList.Count-1; i++)
        {
            Gizmos.DrawLine(pathList[i], pathList[i + 1]);
            Gizmos.DrawWireSphere(pathList[i], nodeDiameter * 0.5f);
        }
        if(pathList.Count > 0)
            Gizmos.DrawWireSphere(pathList[pathList.Count-1], nodeDiameter * 0.5f);
    }
}

public static class Perlin
{
    [System.Serializable]
    public class PerlinConfig
    {
        public string Seed; // Unique identifier for reproducibility
        public int Width;
        public int Height;
        public float Min = 0f;
        public float Max = 0f;
        public int Octaves = 4; // Number of octaves
        public Vector2 Offset; // User-defined offset for sampling
        public float Scale = 50f; // Scale of the Perlin noise
        public float Persistence = 0.5f; // Amplitude reduction for each octave
        public float Lacunarity = 2f; // Frequency increase for each octave
        public float StartInX = 0f;
        public float EndInX = 0f;

        // Sample Perlin noise at a 2D vector
        public float Sample(Vector2 v2) => Sample((int)v2.x, (int)v2.y);

        // Sample Perlin noise at a 2D integer vector
        public float Sample(Vector2Int v2Int) => Sample(v2Int.x, v2Int.y);

        // Sample Perlin noise at specific x and y coordinates
        public float Sample(int x, int y)
        {
            // Compute a seed offset for consistent results across runs
            Vector2 seedOffset = GetSeedOffset();

            float total = 0f;      // Accumulated noise value
            float amplitude = 1f; // Starting amplitude
            float frequency = 1f; // Starting frequency
            float maxValue = 0f;  // Used for normalization

            // Generate octaved Perlin noise
            for (int i = 0; i < Octaves; i++)
            {
                // Apply scale, frequency, and offsets
                float sampleX = (x + seedOffset.x + Offset.x) / Scale * frequency;
                float sampleY = (y + seedOffset.y + Offset.y) / Scale * frequency;

                // Sample Perlin noise
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                // Accumulate the result
                total += perlinValue * amplitude;

                // Track maximum possible value for normalization
                maxValue += amplitude;

                // Adjust amplitude and frequency for next octave
                amplitude *= Persistence; // Reduce amplitude
                frequency *= Lacunarity;  // Increase frequency
            }

            // Normalize the result to 0-1 range
            float normalizedValue = total / maxValue;
            normalizedValue = normalizedValue.Remap(Min, Max, 0f, 1f);

            float xMult = ((float)x).Remap(StartInX, EndInX, 0f, 1f);
            normalizedValue *= xMult;

            return normalizedValue;
        }

        // Generate a seed-based offset to introduce randomness
        private Vector2 GetSeedOffset()
        {
            Random.InitState(Seed.GetHashCode());
            float x = Random.Range(-1000f, 1000f); // Use a large range for better separation
            float y = Random.Range(-1000f, 1000f);
            return new Vector2(x, y);
        }
    }
}