using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    public T[,] data;

    public Matrix(int width, int height)
    {
        data = new T[width, height];

        Width = width;
        Height = height;
        Capacity = data.Length;
    }

	public Matrix(T[,] copyFrom)
    {

        Width = copyFrom.GetLength(0);
        Height = copyFrom.GetLength(1);
        Capacity = copyFrom.Length;

        data = new T[Width, Height];

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                data[i, j] = copyFrom[i, j];
    }

	public Matrix<T> Clone()
    {
        Matrix<T> aux = new Matrix<T>(Width, Height);

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                aux[i, j] = data[i, j];

        return aux;
    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item)
    {
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item

        for (int i = x0; i <= x1; i++)
            for (int j = y0; j <= y1; j++)
                data[i, j] = item;
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1)
    {
        List<T> l = new List<T>();

        for (int i = x0; i <= x1; i++)
            for (int j = y0; j <= y1; j++)
                l.Add(data[i, j]);

        return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y] { get { return data[x, y]; } set { data[x, y] = value; } }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Capacity { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in data)
            yield return item;
    }

	IEnumerator IEnumerable.GetEnumerator()
    {
		return GetEnumerator();
	}
}
