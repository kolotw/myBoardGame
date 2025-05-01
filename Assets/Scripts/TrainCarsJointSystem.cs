using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarsJointSystem : MonoBehaviour
{
    public Transform trainHead; // �w�s�b�������Y
    public GameObject trainCarPrefab; // ���[�w�s��
    public int carCount = 5; // ���[�ƶq
    public float distanceBetweenCars = 2.0f; // ���[���Z

    private List<GameObject> trainCars = new List<GameObject>();

    void Start()
    {
        if (trainHead == null)
        {
            Debug.LogError("�Ы��w�����Y����I");
            return;
        }

        CreateTrainCarsWithJoints();
    }

    void CreateTrainCarsWithJoints()
    {
        // �T�O�����Y��Rigidbody
        Rigidbody headRigidbody = trainHead.GetComponent<Rigidbody>();
        if (headRigidbody == null)
        {
            headRigidbody = trainHead.gameObject.AddComponent<Rigidbody>();
            // �ѩ�����Y��NavMeshAgent����A�NRigidbody�]���B�ʾǪ�
            headRigidbody.isKinematic = true;
            headRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // �O���e�@�`���[/�����Y
        Transform previousTransform = trainHead;
        Rigidbody previousRigidbody = headRigidbody;

        // �Ыب��[�å����`�s��
        for (int i = 0; i < carCount; i++)
        {
            // �b�e�@�`���ͦ��s���[
            Vector3 carPosition = previousTransform.position - previousTransform.forward * distanceBetweenCars;
            GameObject car = Instantiate(trainCarPrefab, carPosition, previousTransform.rotation);
            trainCars.Add(car);

            // �T�O���[��Rigidbody
            Rigidbody carRigidbody = car.GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                carRigidbody = car.AddComponent<Rigidbody>();
                // �]�m�A�X�����z�ݩ�
                carRigidbody.mass = 100;
                carRigidbody.linearDamping = 1.0f;
                carRigidbody.angularDamping = 5.0f;
                carRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // �K�[���`����e���[
            ConfigurableJoint joint = car.AddComponent<ConfigurableJoint>();

            // �]�m���`�s����e�@�`
            joint.connectedBody = previousRigidbody;

            // �]�m���`�Ѽ�
            SetupTrainJoint(joint);

            // ���e���[�����U�@�`��"�e�@�`"
            previousTransform = car.transform;
            previousRigidbody = carRigidbody;
        }
    }

    void SetupTrainJoint(ConfigurableJoint joint)
    {
        // �]�m�s���I
        joint.anchor = new Vector3(0, 0, distanceBetweenCars / 2); // ���[�e��
        joint.connectedAnchor = new Vector3(0, 0, -distanceBetweenCars / 2); // �s����e�@�`�����

        // ��m�ۥѫ�
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        // ����ۥѫ�
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Limited; // ���\Y�b����]��V�^

        // �u�ʭ���]��m�^
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 0.1f; // �i���ʽd��]�̡^
        joint.linearLimit = limit;

        // ���׭���
        SoftJointLimit angularLimit = new SoftJointLimit();
        angularLimit.limit = 30.0f; // �̤j���ס]�ס^

        joint.highAngularXLimit = angularLimit;
        joint.lowAngularXLimit = angularLimit;
        joint.angularYLimit = angularLimit;

        // �]�m�u®�M�����]��m�^
        JointDrive drive = new JointDrive();
        drive.positionSpring = 50.0f;
        drive.positionDamper = 50.0f;
        drive.maximumForce = 100000.0f;

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;

        // �]�m�u®�M�����]���ס^
        JointDrive angularDrive = new JointDrive();
        angularDrive.positionSpring = 10.0f;
        angularDrive.positionDamper = 50.0f;
        angularDrive.maximumForce = 10000.0f;

        joint.angularXDrive = angularDrive;
        joint.angularYZDrive = angularDrive;

        // �����I���M�]�m�L���_���O
        joint.enableCollision = false;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        // ��v�]�m�]���Uí�w�����^
        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.projectionDistance = 0.1f;
        joint.projectionAngle = 10f;
    }
}