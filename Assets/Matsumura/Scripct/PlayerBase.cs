using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

// using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.InputSystem;
public class PlayerBase : CharaBase
{
    /*
        static readonly float GROUNDED_THRESHOLD_TIME = 0.25f;

        [SerializeField] private float m_moveSpeed = 8f;
        [SerializeField] private float m_jumpSpeed = 10f;
        [SerializeField] private float m_shotSpeed = 20f;
        [SerializeField] private Transform m_muzzleTF;
        [SerializeField] private CameraControl m_cameraCont;
        private CharacterController m_charaCont;
        private int m_upperLayerIndex;
        private Animator m_animator;
        private Camera m_camera;
        private Dictionary<string, bool> m_animatorParameterCache;

        private Vector3 m_moveVelocity;
        private Vector3 m_cursorPos;
        private bool m_isPlayingJumpAnim;
        private enum State
        {
            Idle,
            Attack,
            Jump,
            Fall,
            Damage,
            Death,
        }
        private State m_state = State.Idle;
        private int m_stateStep;
        private float m_elapsedTime;

        private float m_fallElapsedTime;
        private float m_groundElapsedTime;

        protected override void Awake()
        {
            base.Awake();
            m_camera = Camera.main;
            m_charaCont = GetComponent<CharacterController>();
            m_animator = GetComponent<Animator>();
            m_upperLayerIndex = m_animator.GetLayerIndex("Upper Layer");
        }

        private void ChangeState(State state)
        {
            if (m_state == state) return;

            if (m_state == State.Attack)
            {
                AttackEnd();
            }
            m_state = state;
            m_stateStep = 0;
            m_elapsedTime = 0f;
            m_fallElapsedTime = 0f;
        }
        private float GetHorizontalInput()
        {
            float axis = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(axis) > 0f)
            {
                return axis;
            }
            if (Keyboard.current != null)
            {
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) return 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) return -1f;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return -1f;
            return 0f;
        }

        private float GetVerticalInput()
        {
            float axis = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(axis) > 0f)
            {
                return axis;
            }
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) return 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) return -1f;
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return -1f;
            return 0f;
        }

        private bool UpdateMove(float moveSpeed)
        {
            float horizontal = GetHorizontalInput();
            float vertical = GetVerticalInput();
            float moveY = m_moveVelocity.y;

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            if (move.sqrMagnitude > 0f)
            {
                m_moveVelocity = move.normalized * moveSpeed;
                m_moveVelocity.y = moveY;
                return true;
            }
            else
            {
                m_moveVelocity = new Vector3(0f, moveY, 0f);
                return false;
            }
        }

        private void CheakGrounded()
        {
            if (m_charaCont.isGrounded)
            {
                m_groundElapsedTime = Time.deltaTime;
                m_fallElapsedTime = 0f;
            }
            else
            {
                m_groundElapsedTime = 0f;
                m_fallElapsedTime += Time.deltaTime;
            }
        }
        private void StateIdle()
        {
            if (m_fallElapsedTime >= GROUNDED_THRESHOLD_TIME)
            {
                ChangeState(State.Fall);
            }
            else if (IsJumpPressed())
            {
                ChangeState(State.Jump);
            }

            else if (IsAttackPressed())
            {
                ChangeState(State.Attack);
            }

            bool isMove = UpdateMove(m_moveSpeed);

            if (isMove)
            {
                Vector3 moveDir = m_moveVelocity;
                moveDir.y = 0f;
                transform.LookAt(transform.position + moveDir);
            }

            SetAnimatorBool("IsMove", isMove);

            Vector3 dir = transform.InverseTransformDirection(m_moveVelocity.normalized);

            SetAnimatorFloat("DirX", dir.x);
            SetAnimatorFloat("DirZ", dir.z);
        }

        //private void OnShot()
        //{
        //    Bullet bullet = Instantiate(m_bulletPrehab, m_muzzleTF.position, m_muzzleTF.rotation);
        //    bullet.Shot(m_shotSpeed);
        //}

        private void AttackStart()
        {
            m_animator.SetLayerWeight(m_upperLayerIndex, 1f);
            SetAnimatorBool("IsAttack", true);
        }
        private void AttackEnd()
        {
            m_animator.SetLayerWeight(m_upperLayerIndex, 0f);
            SetAnimatorBool("IsAttack", false);
        }

        private void StateAttack()
        {

            if (m_fallElapsedTime >= GROUNDED_THRESHOLD_TIME)
            {
                ChangeState(State.Fall);
                return;
            }
            else if (IsJumpPressed())
            {
                ChangeState(State.Jump);
                return;
            }
            switch (m_stateStep)
            {
                case 0:
                    AttackStart();
                    m_stateStep++;
                    break;
                case 1:
                    if (!IsAttackPressed())
                    {
                        ChangeState(State.Idle);
                        return;
                    }
                    break;
            }

            bool isMove = UpdateMove(m_moveSpeed * 0.5f);
            SetAnimatorBool("IsMove", isMove);

            Vector3 lookDir = m_cursorPos - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0f)
            {
                transform.LookAt(transform.position + lookDir);
            }
            Vector3 dir = transform.InverseTransformDirection(m_moveVelocity.normalized);
            SetAnimatorFloat("DirX", dir.x);
            SetAnimatorFloat("DirZ", dir.z);
        }



        private void JumpAnimEnd()
        {
            m_isPlayingJumpAnim = false;
        }

        private void StateJump()
        {
            switch (m_stateStep)
            {
                case 0:
                    m_moveVelocity.y = m_jumpSpeed;
                    m_isPlayingJumpAnim = true;
                    SetAnimatorBool("IsJump", true);
                    m_stateStep++;
                    break;
                case 1:
                    if (m_charaCont.isGrounded || m_moveVelocity.y < 0f)
                    {
                        ChangeState(State.Fall);
                        return;

                    }
                    break;
            }
            bool isMove = UpdateMove(m_moveSpeed);
            if (isMove)
            {
                Vector3 moveDir = m_moveVelocity;
                moveDir.y = 0f;
                transform.LookAt(transform.position + moveDir);
            }
        }
        private void StateFall()
        {
            switch (m_stateStep)
            {
                case 0:
                    if (!m_charaCont.isGrounded && !m_isPlayingJumpAnim)
                    {
                        SetAnimatorBool("IsJump", true);
                        m_isPlayingJumpAnim = true;
                    }
                    m_stateStep++;
                    break;

                case 1:
                    if (m_charaCont.isGrounded)
                    {
                        SetAnimatorBool("IsJump", false);
                        m_isPlayingJumpAnim = false;

                        m_moveVelocity = new Vector3(0f, m_moveVelocity.y, 0f);
                        m_stateStep++;
                    }
                    break;
                case 2:
                    if (!m_isPlayingJumpAnim)
                    {
                        ChangeState(State.Idle);
                    }
                    break;
            }
            if (m_stateStep < 2)
            {
                bool isMove = UpdateMove(m_moveSpeed);
                if (isMove)
                {
                    Vector3 moveDir = m_moveVelocity;
                    moveDir.y = 0f;
                    transform.LookAt(transform.position + moveDir);
                }
            }

        }
        private void StateDamage()
        {

        }
        private void StateDeath()
        {

        }
        private bool IsJumpPressed()
        {
            if (Input.GetButtonDown("Jump")) return true;
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
            return false;
        }

        private bool AnimatorHasParameter(string param)
        {
            if (m_animator == null) return false;
            if (m_animatorParameterCache == null)
            {
                m_animatorParameterCache = new Dictionary<string, bool>();
            }
            if (m_animatorParameterCache.TryGetValue(param, out bool exists))
            {
                return exists;
            }
            foreach (var p in m_animator.parameters)
            {
                if (p.name == param)
                {
                    m_animatorParameterCache[param] = true;
                    return true;
                }
            }
            m_animatorParameterCache[param] = false;
            return false;
        }

        private void SetAnimatorBool(string param, bool value)
        {
            if (AnimatorHasParameter(param))
            {
                m_animator.SetBool(param, value);
            }
        }

        private void SetAnimatorFloat(string param, float value)
        {
            if (AnimatorHasParameter(param))
            {
                m_animator.SetFloat(param, value);
            }
        }

        private bool IsAttackPressed()
        {
            if (Input.GetButton("Fire1")) return true;
            if (Mouse.current != null && Mouse.current.leftButton.isPressed) return true;
            return false;
        }

        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            if (Mouse.current != null)
            {
                Vector2 pos = Mouse.current.position.ReadValue();
                mousePos = new Vector3(pos.x, pos.y, 0f);
            }
            Ray ray = m_camera.ScreenPointToRay(mousePos);
            int layerMask = LayerMask.GetMask("Field");
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                m_cursorPos = hit.point;
                m_cameraCont.SetCursorPos(m_cursorPos);
            }

            switch (m_state)
            {

                case State.Idle: StateIdle(); break;
                case State.Attack: StateAttack(); break;
                case State.Jump: StateJump(); break;
                case State.Fall: StateFall(); break;
                case State.Damage: StateDamage(); break;
                case State.Death: StateDeath(); break;
            }
            if (!m_charaCont.isGrounded)
            {
                m_moveVelocity.y += Physics.gravity.y * Time.deltaTime;
            }
            else if (m_groundElapsedTime >= GROUNDED_THRESHOLD_TIME)
            {
                m_moveVelocity.y = 0f;
            }
            m_charaCont.Move(m_moveVelocity * Time.deltaTime);

            CheakGrounded();
        }
        */
}

