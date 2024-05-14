import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './AdminPage.css';

const AdminPage = () => {
  const [manufacturers, setManufacturers] = useState([]);
  const [models, setModels] = useState([]);
  const [manufacturerName, setManufacturerName] = useState('');
  const [modelName, setModelName] = useState('');
  const [selectedManufacturerId, setSelectedManufacturerId] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const manufacturersResponse = await axios.get('http://localhost:5045/api/Manufacturers');
        if (manufacturersResponse.data && Array.isArray(manufacturersResponse.data.$values)) {
          setManufacturers(manufacturersResponse.data.$values);
        } else {
          console.error('Manufacturers data is not in the expected format:', manufacturersResponse.data);
          setError('Failed to load manufacturers');
        }
        const modelsResponse = await axios.get('http://localhost:5045/api/Models');
        if (modelsResponse.data && Array.isArray(modelsResponse.data.$values)) {
          setModels(modelsResponse.data.$values);
        } else {
          console.error('Models data is not in the expected format:', modelsResponse.data);
          setError('Failed to load models');
        }
      } catch (error) {
        console.error('Error fetching data:', error);
        setError('Failed to load data');
      }
      setLoading(false);
    };

    fetchData();
  }, []);

  const handleAddManufacturer = async () => {
    if (!manufacturerName.trim()) {
      alert('Please enter a manufacturer name');
      return;
    }
    try {
      const response = await axios.post('http://localhost:5045/api/Manufacturers', {
        name: manufacturerName.trim()
      });
      setManufacturers(prev => [...prev, response.data]);
      setManufacturerName('');
    } catch (error) {
      console.error('Error adding manufacturer:', error);
      alert('Failed to add manufacturer');
    }
  };

  const handleAddModel = async () => {
    if (!selectedManufacturerId) {
      alert('Please select a manufacturer');
      return;
    }
    if (!modelName.trim()) {
      alert('Please enter a model name');
      return;
    }
    try {
      const response = await axios.post('http://localhost:5045/api/Models', {
        name: modelName.trim(),
        manufacturerId: parseInt(selectedManufacturerId)
      });
      setModels(prev => [...prev, response.data]);
      setModelName('');
      setSelectedManufacturerId('');
    } catch (error) {
      console.error('Error adding model:', error);
      alert('Failed to add model');
    }
  };

  const handleEditManufacturer = async (id, newName) => {
    try {
      await axios.put(`http://localhost:5045/api/Manufacturers/${id}`, { id,name: newName });
      setManufacturers(prevManufacturers =>
        prevManufacturers.map(manufacturer =>
          manufacturer.id === id ? { ...manufacturer, name: newName } : manufacturer
        )
      );
    } catch (error) {
      console.error('Error editing manufacturer:', error);
      alert('Failed to edit manufacturer');
    }
  };

  const handleDeleteManufacturer = async (id) => {
    try {
      await axios.delete(`http://localhost:5045/api/Manufacturers/${id}`);
      setManufacturers(prevManufacturers =>
        prevManufacturers.filter(manufacturer => manufacturer.id !== id)
      );
    } catch (error) {
      console.error('Error deleting manufacturer:', error);
      alert('Failed to delete manufacturer');
    }
  };

  const handleEditModel = async (id, newName, manufacturerId) => {
    try {
        const body = {
            Id: id,
            Name: newName,
            ManufacturerId: manufacturerId,
        };

        const response = await axios.put(`http://localhost:5045/api/Models/${id}`, body, {
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // Обновляем состояние моделей после успешного запроса
        setModels(prevModels => prevModels.map(model => {
            if (model.id === id) {
                // Возвращаем обновленные данные модели
                return { ...model, name: newName, manufacturerId: manufacturerId };
            } else {
                // Возвращаем неизмененные данные остальных моделей
                return model;
            }
        }));

        console.log('Model updated successfully:', response.data);
    } catch (error) {
        console.error('Error editing model:', error);
        // Обработайте ошибку соответствующим образом
    }
};

  const handleDeleteModel = async (id) => {
    try {
      await axios.delete(`http://localhost:5045/api/Models/${id}`);
      setModels(prevModels => prevModels.filter(model => model.id !== id));
    } catch (error) {
      console.error('Error deleting model:', error);
      alert('Failed to delete model');
    }
  };

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p>Error: {error}</p>;
  }

  return (
    <div className="admin-page">
        <h1 className="admin-header">Admin Page</h1>
        <div>
            <h3 className="admin-header">Add a New Manufacturer</h3>
            <input
                className="input-field"
                type="text"
                placeholder="Manufacturer Name"
                value={manufacturerName}
                onChange={e => setManufacturerName(e.target.value)}
            />
            <button className="button" onClick={handleAddManufacturer}>Add Manufacturer</button>
        </div>
        <div>
            <h3 className="admin-header">Add a New Model</h3>
            <select
                className="input-field"
                value={selectedManufacturerId}
                onChange={e => setSelectedManufacturerId(e.target.value)}
            >
                <option value="">Select Manufacturer</option>
                {manufacturers.map(manufacturer => (
                    <option key={manufacturer.id} value={manufacturer.id}>
                        {manufacturer.name}
                    </option>
                ))}
            </select>
            <input
                className="input-field"
                type="text"
                placeholder="Model Name"
                value={modelName}
                onChange={e => setModelName(e.target.value)}
            />
            <button className="button" onClick={handleAddModel}>Add Model</button>
        </div>
        <div>
            <h3 className="admin-header">Current Manufacturers</h3>
            <ul className="list">
                {manufacturers.map(manufacturer => (
                    <li className="list-item" key={manufacturer.id}>
                        {manufacturer.name}
                        <div>
                            <button className="button" onClick={() => handleEditManufacturer(manufacturer.id, prompt('Enter new name'))}>Edit</button>
                            <button className="button" onClick={() => handleDeleteManufacturer(manufacturer.id)}>Delete</button>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
        <div>
            <h3 className="admin-header">Current Models</h3>
            <ul className="list">
                {models.map(model => (
                    <li className="list-item" key={model.id}>
                        {model.name}
                        <div>
                            <button className="button" onClick={() => handleEditModel(model.id, prompt('Enter new name'))}>Edit</button>
                            <button className="button" onClick={() => handleDeleteModel(model.id)}>Delete</button>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    </div>
);
};

export default AdminPage;
