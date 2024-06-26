﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ThirdPersonFramework.UserInterface;
using System;

public class RacingHUD : BaseUI
{
    public static RacingHUD Instance;
    const float MAX_REV_ANGLE = -135;
    const float ZERO_REV_ANGLE = 135;

    float m_MaxRev, currentRev;

    [SerializeField] Transform m_RevLevelTemplate, m_TachNeedle;
    [SerializeField] TMP_Text m_Speedometer, m_GearIndicator;
    [SerializeField] float m_RevNeedleResponseSpeed = 6.0F;

    [Header("Extras")]
    public GameObject m_BoostGauge;
    public GameObject m_NitrousGauge;
    public Transform m_BoostGaugeLevelTemplate, m_BoostNeedle;
    private float m_MaxBoost;
    private VehicleForcedInduction m_ForcedInduction;

    [Header("Mini Map")]
    [SerializeField] GameObject m_MiniMapCameraPrefab;
    private Vehicle playerVehicle;
    public GameObject m_VehicleIconTemplatePrefab;
    private List<Transform> vehicleIcons;
    [SerializeField][Range(1, 500)] 
    float   m_MiniMapMinScale,
            m_MiniMapMaxScale,
            m_SpeedAtMaxScale, 
            m_MiniMapHeight;

    private Camera m_MiniMapCamera;

    //Flags
    bool m_bHUDInitialised;

    public TMP_Text GetGearIndicator() => m_GearIndicator;
    public TMP_Text GetSpeedometer() => m_Speedometer;
    public void SetGearIndicator(string gear) => m_GearIndicator.text = gear;
    public void SetSpeedometer(float speed) => m_Speedometer.text = Mathf.Floor(speed) + "KM/H";
    public void SetMaxRev(float MaxRev) => m_MaxRev = MaxRev;

    protected override void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        base.Awake();
    }

    protected override void Start()
    {
        InitialiseRacingHUD();
        Hide();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PauseMenu.OnPauseMenuOpened -= OnGamePaused;
        PauseMenu.OnPauseMenuClosed -= OnGameResumed;
        Player.OnPlayerInitialised -= OnPlayerInitialised;
        CameraController.OnIdleCamera -= OnIdleCamera;
        CameraController.OnOutIdleCamera -= OnOutOfIdleCamera;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        Player.OnPlayerInitialised += OnPlayerInitialised;
        PauseMenu.OnPauseMenuClosed += OnGameResumed;
        CameraController.OnIdleCamera += OnIdleCamera;
        CameraController.OnOutIdleCamera += OnOutOfIdleCamera;
    }

    private void OnOutOfIdleCamera() => Show();
    private void OnIdleCamera() => Hide();

    private void OnPlayerInitialised() => InitialiseRacingHUD();
    
    //Called when this game is paused.
    protected override void OnGamePaused()
    {
        base.OnGamePaused();
        if (!m_bHUDInitialised)
            InitialiseRacingHUD();
        Hide();
    }

    protected override void OnGameResumed()
    {
        base.OnGameResumed();
        if (!m_bHUDInitialised)
            InitialiseRacingHUD();
        Show();
    }

    //Called when we init and/or when we resume a game from pause.
    public void InitialiseRacingHUD()
    {
        if (Player.Instance.Vehicle == null)
            return;

        if (!m_bHUDInitialised)
        {
            InitialiseRevCounter();
            FindPlayerAndAttachMiniMap();
            //InitialiseVehicleIcons();
            InitialiseBoostGauge();
            //TODO : SHOW NITRO GAUGE.

            m_bHUDInitialised = true;
        }

        Show();
    }

    private void InitialiseVehicleIcons()
    {
        vehicleIcons = new();

        /*Racer[] racers = FindObjectsOfType<Racer>();

        if (racers.Length <= 0)
            return;

        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i] is null)
                continue;

            GameObject GOIcon = Instantiate(
                    m_VehicleIconTemplatePrefab,
                    racers[i].Vehicle.transform.position + Vector3.up,
                    racers[i].Vehicle.transform.rotation,
                    racers[i].Vehicle.transform
                    );

            Transform icon;

            if (GOIcon != null && GOIcon.TryGetComponent<Transform>(out var iconTransform))
                icon = iconTransform;
            else
            {
                Debug.LogError("Icon Instantiating failed!");
                return;
            }

            if (icon.TryGetComponent<SpriteRenderer>(out var sprite))
            {

                sprite.color = i switch
                {
                    0 => Color.red,
                    1 => Color.green,
                    2 => Color.yellow,
                    3 => Color.blue,
                    4 => Color.black,
                    5 => Color.magenta,
                    6 => Color.cyan,
                    7 => Color.white,
                    8 => Color.gray,
                    _ => Color.white
                };
            }
            else
                Debug.LogError("There is no sprite renderer attached to the vehicle Icon Prefab!");

            icon.eulerAngles = Vector3.right * 90 + Vector3.up * 180;

            icon.gameObject.SetActive(true);

            vehicleIcons.Add(icon);
        }
        */
    }

    private void InitialiseRevCounter()
    {
        currentRev = 0;
        m_MaxRev = 10000;

        m_RevLevelTemplate.gameObject.SetActive(false);

        CreateRevLabels();
    }

    private void InitialiseBoostGauge()
    {
        if (Player.Instance == null)
            return;

        //Show boost gauge if vehicle has some forced induction (turbo/supercharger)
        if (Player.Instance.Vehicle.TryGetComponent<VehicleForcedInduction>(out var forcedInduction))
        {
            m_ForcedInduction = forcedInduction;
            m_MaxBoost = m_ForcedInduction.GetMaxBoost();
            CreateBoostGaugeLevels();
            m_BoostGaugeLevelTemplate.gameObject.SetActive(false);
            m_BoostGauge.SetActive(true);
        }
        else
        {   //Don't show boost gauge.
            m_BoostGauge.SetActive(false);
            return;
        }
    }

    private void FindPlayerAndAttachMiniMap()
    {
        m_MiniMapCamera = Instantiate(m_MiniMapCameraPrefab, transform.position, Quaternion.identity).GetComponent<Camera>();
        playerVehicle = Player.Instance.Vehicle;
        TrackPlayerWithMiniMapCamera();
    }

    public void SetRev(float currentRev)
    {
        this.currentRev = currentRev;

        if (this.currentRev > m_MaxRev) this.currentRev = m_MaxRev;
    }

    private void CreateBoostGaugeLevels()
    {
        int labelCount = 3;

        float totalAngleSize = ZERO_REV_ANGLE - 0;

        for (int i = 0; i <= labelCount; i++)
        {
            Transform boostLevelTransform = Instantiate(m_BoostGaugeLevelTemplate, m_BoostNeedle.parent);

            float boostLabelAngle = ZERO_REV_ANGLE - ((float)i / labelCount) * totalAngleSize;

            //Boost gauge doesn't show bar (that's just not needed for this type of game, its not a simulator)

            boostLevelTransform.eulerAngles = new(0, 0, boostLabelAngle);

            boostLevelTransform.gameObject.SetActive(true);
        }
    }

    private void CreateRevLabels()
    {
        int labelCount = 10;

        float totalAngleSize = ZERO_REV_ANGLE - MAX_REV_ANGLE;

        for (int i = 0; i <= labelCount; i++)
        {
            Transform revLabelTransform = Instantiate(m_RevLevelTemplate, m_TachNeedle.parent);

            float labelRevNormalised = (float)i / labelCount;

            float revLabelAngle = ZERO_REV_ANGLE - labelRevNormalised * totalAngleSize;

            revLabelTransform.eulerAngles = new Vector3(0, 0, revLabelAngle);

            revLabelTransform.GetComponentInChildren<TMP_Text>().text = Mathf.RoundToInt(labelRevNormalised * m_MaxRev / 1000.0F).ToString();

            revLabelTransform.GetComponentInChildren<TMP_Text>().transform.eulerAngles *= 0;

            revLabelTransform.gameObject.SetActive(true);
        }
    }


    protected override void Update()
    {
        if (IsHidden() || Player.Instance && Player.Instance.Vehicle == null)
            return;

        HandleRevNeedle();

        TrackPlayerWithMiniMapCamera();

        if (m_BoostGauge.activeSelf)
            HandleBoostNeedle();
    }

    private void HandleBoostNeedle()
    {
        float angle = m_BoostNeedle.eulerAngles.z;

        float targetAngle = GetBoostRotation();

        float finalAngle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * 10.0F);

        m_BoostNeedle.eulerAngles = new Vector3(0, 0, finalAngle);
    }

    private void HandleRevNeedle()
    {
        float angle = m_TachNeedle.eulerAngles.z;

        float targetAngle = GetRevRotation();

        float finalAngle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * m_RevNeedleResponseSpeed);

        m_TachNeedle.eulerAngles = new Vector3(0, 0, finalAngle);
    }

    private void TrackPlayerWithMiniMapCamera()
    {
        m_MiniMapCamera.transform.position = playerVehicle.transform.position + Vector3.up * m_MiniMapHeight;
        m_MiniMapCamera.transform.eulerAngles = Vector3.right * 90 + Vector3.forward * playerVehicle.transform.eulerAngles.y;
        m_MiniMapCamera.orthographicSize = Mathf.Lerp(m_MiniMapMinScale, m_MiniMapMaxScale, playerVehicle.SpeedKMH / m_SpeedAtMaxScale);
    }

    private float GetRevRotation()
    {
        float totalAngleSize = ZERO_REV_ANGLE - MAX_REV_ANGLE;

        if (m_MaxRev <= 0)
            m_MaxRev = 8000;

        float revNormalised = currentRev / m_MaxRev;

        return ZERO_REV_ANGLE - revNormalised * totalAngleSize;
    }

    private float GetBoostRotation()
    {
        float totalAngleSize = ZERO_REV_ANGLE - 0;

        if (m_ForcedInduction.GetMaxBoost() <= 0 || !m_ForcedInduction)
            return 0;

        float boostNormalised = m_ForcedInduction.GetCurrentBoost() / m_ForcedInduction.GetMaxBoost();

        return ZERO_REV_ANGLE - boostNormalised * totalAngleSize;
    }
}