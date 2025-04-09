"use client";
import React, {useEffect, useState} from 'react';
import axios from 'axios';

interface BusinessCard {
    id: string;
    fullName: string;
    title: string;
    email: string;
    phone: string;
    companyName: string;
    website: string;
    address: string;
  }
  
  interface PaginationMetadata {
    CurrentPage: number;
    TotalPages: number;
    PageSize: number;
    TotalCount: number;
    HasPrevious: boolean;
    HasNext: boolean;
  }

export default function ManageCard() {
    return (
        <div>
            <h1 className="text-2xl font-bold mb-4">Manage Business Cards</h1>
            <p className="text-gray-600">CRUD Functionality coming soon..</p>
        </div>
    )
}