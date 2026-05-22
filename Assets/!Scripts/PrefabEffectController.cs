using UnityEngine;

public class PrefabEffectController : MonoBehaviour
{
    [SerializeField] private GameObject effect1Prefab1;
    private GameObject currentEffectInstance1;

    [SerializeField] private GameObject effect2Prefab2;
    private GameObject currentEffectInstance2;

    [SerializeField] private GameObject effect3Prefab3;
    private GameObject currentEffectInstance3;


    // BONUSES
    [SerializeField] private GameObject bonusEffectPrefab1;
    private GameObject bonusEffectInstance1;

    [SerializeField] private GameObject bonusEffectPrefab2;
    private GameObject bonusEffectInstance2;

    [SerializeField] private GameObject bonusEffectPrefab3;
    private GameObject bonusEffectInstance3;


    // Создание и запуск обычного 1 эффекта
    public void SpawnAndPlayEffect1(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (currentEffectInstance1 != null)
        {
            Destroy(currentEffectInstance1);
        }

        // Создаем новый экземпляр
        currentEffectInstance1 = Instantiate(effect1Prefab1, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(currentEffectInstance1, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(currentEffectInstance1, 3f);
    }

    // Создание и запуск обычного 2 эффекта
    public void SpawnAndPlayEffect2(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (currentEffectInstance2 != null)
        {
            Destroy(currentEffectInstance2);
        }

        // Создаем новый экземпляр
        currentEffectInstance2 = Instantiate(effect2Prefab2, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(currentEffectInstance2, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(currentEffectInstance2, 3f);
    }

    // Создание и запуск обычного 3 эффекта
    public void SpawnAndPlayEffect3(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (currentEffectInstance3 != null)
        {
            Destroy(currentEffectInstance3);
        }

        // Создаем новый экземпляр
        currentEffectInstance3 = Instantiate(effect3Prefab3, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(currentEffectInstance3, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(currentEffectInstance3, 3f);
    }



    // Создание и запуск бонус эффекта 1
    public void SpawnAndPlayBonusEffect1(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (bonusEffectInstance1 != null)
        {
            Destroy(bonusEffectInstance1);
        }

        // Создаем новый экземпляр
        bonusEffectInstance1 = Instantiate(bonusEffectPrefab1, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(bonusEffectInstance1, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(bonusEffectInstance1, 3f);
    }

    // Создание и запуск бонус эффекта 2
    public void SpawnAndPlayBonusEffect2(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (bonusEffectInstance2 != null)
        {
            Destroy(bonusEffectInstance2);
        }

        // Создаем новый экземпляр
        bonusEffectInstance2 = Instantiate(bonusEffectPrefab2, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(bonusEffectInstance2, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(bonusEffectInstance2, 3f);
    }

    // Создание и запуск бонус эффекта 3
    public void SpawnAndPlayBonusEffect3(Vector3 position, Quaternion rotation)
    {
        // Уничтожаем предыдущий эффект если есть
        if (bonusEffectInstance3 != null)
        {
            Destroy(bonusEffectInstance3);
        }

        // Создаем новый экземпляр
        bonusEffectInstance3 = Instantiate(bonusEffectPrefab3, position, rotation);

        // Автоматически ищем компоненты для управления
        ManageEffectComponents(bonusEffectInstance3, true);

        // Автоматическое уничтожение через 3 секунды
        Destroy(bonusEffectInstance3, 3f);
    }

    // Остановка и уничтожение эффекта
    public void StopAndDestroyEffect()
    {
        if (currentEffectInstance1 != null)
        {
            ManageEffectComponents(currentEffectInstance1, false);

            // Уничтожаем через 1 секунду (или после завершения)
            Destroy(currentEffectInstance1, 1f);
            currentEffectInstance1 = null;
        }
    }

    private void ManageEffectComponents(GameObject effect, bool play)
    {
        // Управление Particle System
        var particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (play) ps.Play();
            else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // Управление Animator
        var animators = effect.GetComponentsInChildren<Animator>();
        foreach (var animator in animators)
        {
            animator.SetBool("IsPlaying", play);
        }

        // Управление AudioSource
        var audioSources = effect.GetComponentsInChildren<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (play) audioSource.Play();
            else audioSource.Stop();
        }
    }
}