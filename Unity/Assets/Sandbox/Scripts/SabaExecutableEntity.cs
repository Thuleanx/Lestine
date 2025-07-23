using System.Collections.Generic;
using UnityEngine;

using PrettyPatterns;

public class SabaExecutableRuntimeGroup :
	Singleton<SabaExecutableRuntimeGroup> {

	public List<SabaExecutableEntity> activeEdibleEnemies =
		new List<SabaExecutableEntity>();
	public List<SabaExecutableEntity> inactiveEdibleEnemies =
		new List<SabaExecutableEntity>();
}

[RequireComponent(typeof(SabaEntity))]
public class SabaExecutableEntity : MonoBehaviour {
	[SerializeField, Range(0, 1)]
	float threshold;

	SabaEntity entity;

	void Awake() { entity = GetComponent<SabaEntity>(); }

	void OnEnable() {
		SabaExecutableRuntimeGroup.instance.inactiveEdibleEnemies.Add(this);
	}

	void OnDisable() {
		SabaExecutableRuntimeGroup.instance.activeEdibleEnemies.Remove(this);
		SabaExecutableRuntimeGroup.instance.inactiveEdibleEnemies.Remove(this);
	}

	public static void UpdateActiveEntities() {
        List<SabaExecutableEntity> newlyActivatedEntities = new List<SabaExecutableEntity>();
		foreach (SabaExecutableEntity executableEntity in SabaExecutableRuntimeGroup
					 .instance.inactiveEdibleEnemies) {

            SabaEntity entity = executableEntity.entity;
            bool shouldActivate = entity.Resource.Health <
                                    entity.Stats.MaxHealth * executableEntity.threshold;
            if (shouldActivate) newlyActivatedEntities.Add(executableEntity);
        }

        SabaExecutableRuntimeGroup.instance.activeEdibleEnemies.AddRange(newlyActivatedEntities);
        foreach (SabaExecutableEntity entity in newlyActivatedEntities)
            SabaExecutableRuntimeGroup.instance.inactiveEdibleEnemies.Remove(entity);
	}
}
