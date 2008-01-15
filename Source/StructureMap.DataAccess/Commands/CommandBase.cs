using System;
using System.Data;
using System.Runtime.CompilerServices;
using StructureMap.DataAccess.JSON;

namespace StructureMap.DataAccess.Commands
{
    public abstract class CommandBase : IInitializable, IReaderSource, ICommand
    {
        private IDbCommand _innerCommand;
        private string _name;
        private ParameterCollection _parameters;
        private IDataSession _session;

        public CommandBase()
        {
            _parameters = new ParameterCollection();
        }

        public ParameterCollection Parameters
        {
            get { return _parameters; }
        }

        protected IDbCommand innerCommand
        {
            get { return _innerCommand; }
        }

        #region ICommand Members

        public int Execute()
        {
            try
            {
                prepareCommand(_innerCommand);
                return _session.ExecuteCommand(_innerCommand);
            }
            catch (CommandFailureException ex)
            {
                ex.CommandName = Name;
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(Name, _innerCommand, ex);
            }
        }

        #endregion

        #region IInitializable Members

        public abstract void Initialize(IDatabaseEngine engine);

        #endregion

        #region IReaderSource Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [IndexerName("Parameter")]
        public object this[string parameterName]
        {
            get { return _parameters[parameterName].GetProperty(); }
            set { _parameters[parameterName].SetProperty(value); }
        }

        public void Attach(IDataSession session)
        {
            if (_innerCommand == null || _parameters == null)
            {
                session.Initialize(this);
            }

            _session = session;
        }

        public string ExecuteJSON()
        {
            DataSet dataSet = ExecuteDataSet();
            JSONSerializer serializer = new JSONSerializer(dataSet.Tables[0]);
            return serializer.CreateJSON();
        }

        public IDataReader ExecuteReader()
        {
            try
            {
                prepareCommand(_innerCommand);
                return _session.ExecuteReader(_innerCommand);
            }
            catch (CommandFailureException ex)
            {
                ex.CommandName = Name;
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(Name, _innerCommand, ex);
            }
        }

        public DataSet ExecuteDataSet()
        {
            try
            {
                prepareCommand(_innerCommand);
                return _session.ExecuteDataSet(_innerCommand);
            }
            catch (CommandFailureException ex)
            {
                ex.CommandName = Name;
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(Name, _innerCommand, ex);
            }
        }

        public object ExecuteScalar()
        {
            try
            {
                prepareCommand(_innerCommand);
                return _session.ExecuteScalar(_innerCommand);
            }
            catch (CommandFailureException ex)
            {
                ex.CommandName = Name;
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandFailureException(Name, _innerCommand, ex);
            }
        }

        #endregion

        public void OverrideParameterType(string parameterName, DbType dbtype)
        {
            _parameters[parameterName].OverrideParameterType(dbtype);
        }

        protected void initializeMembers(ParameterCollection parameters, IDbCommand innerCommand)
        {
            _parameters = parameters;
            _innerCommand = innerCommand;
        }

        protected virtual void prepareCommand(IDbCommand command)
        {
            // no-op
        }
    }
}