using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointDebug : MonoBehaviour {

	// =============================================================================
	// UTILIDADE DE DEPURACIÓN PARA WAYPOINTS (EXECUTE EN EDIT MODE)
	// =============================================================================
	// Este script renomea automaticamente os GameObjects etiquetados como "wp"
	// e actualiza o TextMesh do marker para mostrar o nome do waypoint pai.
	// Está pensado para uso no editor (facilita ordenar/identificar pasos).

	// Renomea todos os GameObjects con tag "wp" nun formato compacto (WP001, WP002...)
	// Se 'overlook' non é nulo, ese obxecto non se renomeará (permite preservar un seleccionado).
	void RenameWPs(GameObject overlook)
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("wp"); 
		int i = 1;
		foreach (GameObject go in gos)  
		{ 
			if(go != overlook)
			{
				go.name = "WP" + string.Format("{0:000}",i); 
				i++; 
			} 
		}    
	}

	// Se o marker é destruído no editor, renomea os demais para manter orde
	void OnDestroy()
	{
		RenameWPs(this.gameObject);
	}

	// Inicialización no editor/tempo de execución
	void Start () {
		// Só renomeamos se o pai se chama exactamente "WayPoint" (evitar cambios en outros usos)
		if(this.transform.parent.gameObject.name != "WayPoint") return;
		RenameWPs(null);
	}
    
	// Actualiza o TextMesh do marcador para amosar o nome do waypoint pai
	void Update () {
		this.GetComponent<TextMesh>().text = this.transform.parent.gameObject.name;
	}
}
