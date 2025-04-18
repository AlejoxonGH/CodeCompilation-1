﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class Sudoku : MonoBehaviour
{
	public Cell prefabCell;
	public Canvas canvas;
	public Text feedback;
	public float stepDuration = 0.05f;
	[Range(1, 82)]public int difficulty = 40;

	Matrix<Cell> _board;
	Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
	int _smallSide;
	int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();



    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;

    [SerializeField]
    private int size = 3;


    void Start()
    {
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;
        _smallSide = size;
        _bigSide = _smallSide * size;
        frequency = frequency * Mathf.Pow(r, 2);
        CreateEmptyBoard();
        ClearBoard();
    }

    void ClearBoard()
    {
        _createdMatrix = new Matrix<int>(_bigSide, _bigSide);

        for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                _board[x, y].number = 0;
                _board[x, y].locked = _board[x, y].invalid = false;
            }
    }

	void CreateEmptyBoard()
    {
        float spacing = 68f;
		float startX = -spacing * 4f;
		float startY = spacing * 4f;

        _board = new Matrix<Cell>(_bigSide, _bigSide);

        for (int x = 0; x<_board.Width; x++)
			for(int y = 0; y<_board.Height; y++)
            {
                var cell = _board[x, y] = Instantiate(prefabCell);
				cell.transform.SetParent(canvas.transform, false);
				cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
			}
    }

    //IMPLEMENTAR
    int watchdog = 10000;
	bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution)
    {
        Debug.Log(protectMaxDepth); //NUMERO DE INTENTO

        if (protectMaxDepth > watchdog) // SI ME PASO DEL WATCHDOG RETORNO FALSE (NO SE PUDO RESOLVER EL SUDOKU)
            return false;

        if (_board[x, y].locked) // CHEQUEO SI ESTA BLOQUEADO
        {
            if (!CanPlaceValue(matrixParent, _board[x, y].number, x, y))
                return false;

            if (x >= _bigSide - 1)     // SI LO ESTÁ CHEQUEO SI TENGO QUE BAJAR DE FILA
            {
                if (y >= _bigSide - 1) // SI TENGO QUE BAJAR DE FILA CHEQUEO SI ESTOY EN LA ULTIMA CASILLA
                    return true;       // SI ESTOY EN LA ULTIMA DEVUELVO TRUE (SE RESOLVIO EL SUDOKU)

                return RecuSolve(matrixParent, 0, y + 1, protectMaxDepth + 1, solution);   // RETORNO LO QUE ME DEVUELVA LA PROXIMA CASILLA                                                                            
            }
            else                                                                         
                return RecuSolve(matrixParent, x + 1, y, protectMaxDepth + 1, solution);   // RETORNO LO QUE ME DEVUELVA LA PROXIMA CASILLA
        }

        for (int num = 1; num <= _bigSide; num++)
        {
            if (CanPlaceValue(matrixParent, num, x, y))
            {
                matrixParent[x, y] = num;
                solution.Add(new Matrix<int>(matrixParent.data));

                if (x >= _bigSide - 1)     
                {
                    if (y >= _bigSide - 1)
                        return true;

                    if (RecuSolve(matrixParent, 0, y + 1, protectMaxDepth + 1, solution))
                        return true;
                    else
                        matrixParent[x, y] = 0;
                }
                else
                {
                    if (RecuSolve(matrixParent, x + 1, y, protectMaxDepth + 1, solution))
                        return true;
                    else
                        matrixParent[x, y] = 0;
                }
            }
        }

        return false;
    }

    void OnAudioFilterRead(float[] array, int channels)
    {
        if(canPlayMusic)
        {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++)
            {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }
    }

    void changeFreq(int num)
    {
        frequency = 440 + num * 80;
    }

	//IMPLEMENTAR - punto 3
	IEnumerator ShowSequence(List<Matrix<int>> seq)
    {
        for (int i = 0; i < seq.Count; i++)
        {
            yield return new WaitForSeconds(stepDuration);
            TranslateAllValues(seq[i]);
            feedback.text = $"Pasos: {i + 1}/{seq.Count} - {memory} - {canSolve}";
        }
    }

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            SolvedSudoku();
        else if(size <= 3 && (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))) 
            CreateSudoku();	
	}

	//modificar lo necesario para que funcione.
    void SolvedSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 10000;
        var result = RecuSolve(_createdMatrix, 0, 0, 1, solution);
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = $"Pasos: 0/{solution.Count} - {memory} - {canSolve}";
        StartCoroutine(ShowSequence(solution));
    }

    void CreateSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        canPlayMusic = false;
        ClearBoard();
        var l = new List<Matrix<int>>();
        watchdog = 10000;
        GenerateValidLine(_createdMatrix, 0, 0);
        var result = RecuSolve(_createdMatrix, 0, 0, 1, l);
        _createdMatrix = l[l.Count - 1].Clone();
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
        TranslateAllValues(_createdMatrix);
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = $"Pasos: {l.Count}/{l.Count} - {memory} - {canSolve}";
    }

	void GenerateValidLine(Matrix<int> mtx, int x, int y)
	{
		int[]aux = new int[_bigSide];
		for (int i = 0; i < _bigSide; i++) 
			aux [i] = i + 1;

		int numAux = 0;
		for (int j = 0; j < aux.Length; j++) 
		{
			int r = 1 + Random.Range(j,aux.Length);
			numAux = aux [r-1];
			aux [r-1] = aux [j];
			aux [j] = numAux;
		}

		for (int k = 0; k < aux.Length; k++) 
			mtx [k, 0] = aux [k];
	}


	void ClearUnlocked(Matrix<int> mtx)
	{
		for (int i = 0; i < _board.Height; i++)
			for (int j = 0; j < _board.Width; j++)
				if (!_board [j, i].locked)
					mtx[j,i] = Cell.EMPTY;
	}

	void LockRandomCells()
	{
        List<Vector2> posibles = new List<Vector2> ();

        for (int i = 0; i < _board.Height; i++)
			for (int j = 0; j < _board.Width; j++)
                if (!_board[j, i].locked)
                    posibles.Add (new Vector2(j,i));

        for (int k = 0; k < ((_bigSide * _bigSide)+ 1)-difficulty; k++)
        {
            int r = Random.Range (0, posibles.Count);
			_board [(int)posibles [r].x, (int)posibles [r].y].locked = true;
			posibles.RemoveAt (r);
		}
    }

    void TranslateAllValues(Matrix<int> matrix)
    {
        for (int y = 0; y < _board.Height; y++)
            for (int x = 0; x < _board.Width; x++)
                _board[x, y].number = matrix[x, y];
    }

    void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }

    void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
            for (int y = y0; y < yf; y++)
                _board[x, y].number = _createdMatrix[x, y];
    }

    void CreateNew()
    {
        _createdMatrix = new Matrix<int>(Tests.validBoards[1]);
        TranslateAllValues(_createdMatrix);
    }

    bool CanPlaceValue(Matrix<int> mtx, int value, int x, int y)
    {
        List<int> row = new List<int>();
        List<int> column = new List<int>();
        List<int> region = new List<int>();
        List<int> total = new List<int>();

        for (int i = 0; i < mtx.Width; i++)
            if (i != x)
                row.Add(mtx[i, y]);

        for (int j = 0; j < mtx.Height; j++)
            if (j != y)
                column.Add(mtx[x, j]);

        int regionStartX = (x / size) * size;
        int regionStartY = (y / size) * size;
        for (int i = regionStartX; i < regionStartX + size; i++)
            for (int j = regionStartY; j < regionStartY + size; j++)
                if (i != x || j != y)
                    region.Add(mtx[i, j]);

        total.AddRange(row); total.AddRange(column); total.AddRange(region);
        total = FilterZeros(total);

        return !total.Contains(value);
    }


    List<int> FilterZeros(List<int> list)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++)
            if (list[i] != 0) aux.Add(list[i]);

        return aux;
    }
}
