using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarsJointSystem : MonoBehaviour
{
    public Transform trainHead; // 已存在的火車頭
    public GameObject trainCarPrefab; // 車廂預製體
    public int carCount = 5; // 車廂數量
    public float distanceBetweenCars = 2.0f; // 車廂間距

    private List<GameObject> trainCars = new List<GameObject>();

    void Start()
    {
        if (trainHead == null)
        {
            Debug.LogError("請指定火車頭物體！");
            return;
        }

        CreateTrainCarsWithJoints();
    }

    void CreateTrainCarsWithJoints()
    {
        // 確保火車頭有Rigidbody
        Rigidbody headRigidbody = trainHead.GetComponent<Rigidbody>();
        if (headRigidbody == null)
        {
            headRigidbody = trainHead.gameObject.AddComponent<Rigidbody>();
            // 由於火車頭由NavMeshAgent控制，將Rigidbody設為運動學的
            headRigidbody.isKinematic = true;
            headRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // 記錄前一節車廂/火車頭
        Transform previousTransform = trainHead;
        Rigidbody previousRigidbody = headRigidbody;

        // 創建車廂並用關節連接
        for (int i = 0; i < carCount; i++)
        {
            // 在前一節後方生成新車廂
            Vector3 carPosition = previousTransform.position - previousTransform.forward * distanceBetweenCars;
            GameObject car = Instantiate(trainCarPrefab, carPosition, previousTransform.rotation);
            trainCars.Add(car);

            // 確保車廂有Rigidbody
            Rigidbody carRigidbody = car.GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                carRigidbody = car.AddComponent<Rigidbody>();
                // 設置適合的物理屬性
                carRigidbody.mass = 100;
                carRigidbody.drag = 1.0f;
                carRigidbody.angularDrag = 5.0f;
                carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // 添加關節到當前車廂
            ConfigurableJoint joint = car.AddComponent<ConfigurableJoint>();

            // 設置關節連接到前一節
            joint.connectedBody = previousRigidbody;

            // 設置關節參數
            SetupTrainJoint(joint);

            // 當前車廂成為下一節的"前一節"
            previousTransform = car.transform;
            previousRigidbody = carRigidbody;
        }
    }

    void SetupTrainJoint(ConfigurableJoint joint)
    {
        // 設置連接點
        joint.anchor = new Vector3(0, 0, distanceBetweenCars / 2); // 車廂前方
        joint.connectedAnchor = new Vector3(0, 0, -distanceBetweenCars / 2); // 連接到前一節的後方

        // 位置自由度
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        // 旋轉自由度
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Limited; // 允許Y軸旋轉（轉向）

        // 線性限制（位置）
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 0.1f; // 可移動範圍（米）
        joint.linearLimit = limit;

        // 角度限制
        SoftJointLimit angularLimit = new SoftJointLimit();
        angularLimit.limit = 30.0f; // 最大角度（度）

        joint.highAngularXLimit = angularLimit;
        joint.lowAngularXLimit = angularLimit;
        joint.angularYLimit = angularLimit;

        // 設置彈簧和阻尼（位置）
        JointDrive drive = new JointDrive();
        drive.positionSpring = 50.0f;
        drive.positionDamper = 50.0f;
        drive.maximumForce = 100000.0f;

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;

        // 設置彈簧和阻尼（角度）
        JointDrive angularDrive = new JointDrive();
        angularDrive.positionSpring = 10.0f;
        angularDrive.positionDamper = 50.0f;
        angularDrive.maximumForce = 10000.0f;

        joint.angularXDrive = angularDrive;
        joint.angularYZDrive = angularDrive;

        // 關閉碰撞和設置無限斷裂力
        joint.enableCollision = false;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        // 投影設置（幫助穩定模擬）
        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionDistance = 0.1f;
        joint.projectionAngle = 10f;
    }
}