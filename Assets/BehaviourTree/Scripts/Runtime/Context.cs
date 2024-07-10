using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Context
{
    [SerializeField] private GameObject _gameObject;
    public GameObject GameObject { get => _gameObject; set => _gameObject = value; }

    [SerializeField] private Transform _transform;
    public Transform Transform { get => _transform; set => _transform = value; }

    [SerializeField] private Animator _anim;
    public Animator Anim { get => _anim; set => _anim = value; }

    [SerializeField] private Rigidbody _physics;
    public Rigidbody Physics { get => _physics; set => _physics = value; }

    [SerializeField] private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; }

    [SerializeField] private SphereCollider _sphereCollider;
    public SphereCollider SphereCollider { get => _sphereCollider; set => _sphereCollider = value; }

    [SerializeField] private BoxCollider _boxCollider;
    public BoxCollider BoxCollider { get => _boxCollider; set => _boxCollider = value; }

    [SerializeField] private CapsuleCollider _capsuleCollider;
    public CapsuleCollider CapsuleCollider { get => _capsuleCollider; set => _capsuleCollider = value; }

    [SerializeField] private CharacterController _characterController;
    public CharacterController CharacterController { get => _characterController; set => _characterController = value; }

    public static Context CreateFromGameObject(GameObject gameObject)
    {
        Context context = new Context();
        context.GameObject = gameObject;
        context.Transform = gameObject.transform;
        context.Anim = gameObject.GetComponent<Animator>();
        context.Physics = gameObject.GetComponent<Rigidbody>();
        context.Agent = gameObject.GetComponent<NavMeshAgent>();
        context.SphereCollider = gameObject.GetComponent<SphereCollider>();
        context.BoxCollider = gameObject.GetComponent<BoxCollider>();
        context.CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        context.CharacterController = gameObject.GetComponent<CharacterController>();
        return context;
    }
}
