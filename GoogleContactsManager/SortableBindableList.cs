using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleContactsManager
{
    /// <summary>
    /// Inspirado em http://msdn.microsoft.com/en-us/library/aa480736.aspx
    /// Atualizado com http://stackoverflow.com/questions/23661195/datagridview-using-sortablebindinglist
    /// Não mantém a ordenação quando se inserem novos elementos ou se alteram outros.
    /// </summary>
    public class SortableBindableList<T> : BindingList<T>
    {
        public SortableBindableList()
            : base()
        {
        }
        public SortableBindableList(IList<T> list)
            : base(list)
        {
        }
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }
        protected override bool IsSortedCore
        {
            get { return sortPropertyValue != null; }
        }
        PropertyDescriptor sortPropertyValue = null;
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortPropertyValue; }
        }
        ListSortDirection sortDirectionValue;
        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirectionValue; }
        }
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            // só pode ordenar se a propriedade implementar o interface IComparable
            Type interfaceType = prop.PropertyType.GetInterface("IComparable");
            if (interfaceType == null)
            {
                return;
            }
            try
            {
                // para não estar a chamar o evento em cada troca
                RaiseListChangedEvents = false;

                // guarda a propriedade e a direcção
                sortPropertyValue = prop;
                sortDirectionValue = direction;

                if (Items is List<T>)
                {
                    //List<T> lista = (List<T>)Items;
                    //lista.Sort((x, y) =>
                    //{
                    //    IComparable vx = (IComparable)prop.GetValue(x);
                    //    IComparable vy = (IComparable)prop.GetValue(y);
                    //    return vx.CompareTo(vy);
                    //});

                    if (direction == ListSortDirection.Ascending)
                    {
                        ((List<T>)Items).Sort((x, y) => ((IComparable)prop.GetValue(x)).CompareTo((IComparable)prop.GetValue(y)));
                    }
                    else
                    {
                        ((List<T>)Items).Sort((y, x) => ((IComparable)prop.GetValue(x)).CompareTo((IComparable)prop.GetValue(y)));
                    }
                }
                else
                { 
                    IEnumerable<T> listaOrdenada; 
                    if (direction == ListSortDirection.Ascending)
                    {
                        listaOrdenada = Items.OrderBy(i => prop.GetValue(i));
                    }
                    else
                    {
                        listaOrdenada = Items.OrderByDescending(i => prop.GetValue(i));
                    }
                    int newIndex = 0;
                    foreach (object item in listaOrdenada)
                    {
                        Items[newIndex] = (T)item;
                        newIndex++;
                    }
                }
            }
            finally
            {
                RaiseListChangedEvents = true;
            }
            // Notificar alteração efetuada
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
