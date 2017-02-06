// **********************************************************************
//
// Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#include <IceGrid/RegistryAdminRouter.h>
#include <Ice/Ice.h>

using namespace IceGrid;
using namespace Ice;
using namespace std;

namespace
{

class SynchronizationCallbackI : public SynchronizationCallback
{
public:

    SynchronizationCallbackI(const IceUtil::Handle<RegistryServerAdminRouter>& adminRouter,
                             const AMD_Object_ice_invokePtr& cb,
                             const pair<const Byte*, const Byte*>& inParams,
                             const Current& current) :
        _callback(cb), _inParams(inParams.first, inParams.second), _current(current)
    {
    }

    void synchronized()
    {
        //
        // Retry to forward the call.
        //
        _adminRouter->ice_invoke_async(_callback, make_pair(&_inParams[0], &_inParams[0] + _inParams.size()), _current);
    }

    void synchronized(const Ice::Exception& ex)
    {
        _callback->ice_exception(Ice::ObjectNotExistException(__FILE__, __LINE__));
    }

private:

    const IceUtil::Handle<RegistryServerAdminRouter> _adminRouter;
    const AMD_Object_ice_invokePtr _callback;
    const vector<Byte> _inParams;
    const Current _current;
};

}

IceGrid::RegistryServerAdminRouter::RegistryServerAdminRouter(const DatabasePtr& database) :
    _database(database)
{
}

void
IceGrid::RegistryServerAdminRouter::ice_invoke_async(const AMD_Object_ice_invokePtr& cb,
                                                     const std::pair<const Ice::Byte*, const Ice::Byte*>& inParams,
                                                     const Current& current)
{
    ObjectPrx target = 0;

    try
    {
        ServerEntryPtr server = _database->getServer(current.id.name);
        try
        {
            target = server->getAdminProxy();
        }
        catch(const SynchronizationException&)
        {
            server->addSyncCallback(new SynchronizationCallbackI(this, cb, inParams, current));
            return; // Wait for the server synchronization to complete and retry.
        }
    }
    catch(const ServerNotExistException&)
    {
    }
    catch(const NodeUnreachableException&)
    {
    }
    catch(const DeploymentException&)
    {
    }

    if(target == 0)
    {
        throw ObjectNotExistException(__FILE__, __LINE__);
    }

    target = target->ice_facet(current.facet);

    invokeOnTarget(target, cb, inParams, current);
}


IceGrid::RegistryNodeAdminRouter::RegistryNodeAdminRouter(const string& collocNodeName, const DatabasePtr& database) :
    _collocNodeName(collocNodeName),
    _database(database)
{
}

void
IceGrid::RegistryNodeAdminRouter::ice_invoke_async(const AMD_Object_ice_invokePtr& cb,
                                                   const std::pair<const Ice::Byte*, const Ice::Byte*>& inParams,
                                                   const Current& current)
{
    ObjectPrx target;

    if(!_collocNodeName.empty() && current.id.name == _collocNodeName)
    {
        // Straight to the local Admin object
        target = current.adapter->getCommunicator()->getAdmin();
    }
    else
    {
        try
        {
            target = _database->getNode(current.id.name)->getAdminProxy();
        }
        catch(const NodeUnreachableException&)
        {
        }
        catch(const NodeNotExistException&)
        {
        }

        if(target == 0)
        {
            throw ObjectNotExistException(__FILE__, __LINE__);
        }
    }

    target = target->ice_facet(current.facet);

    invokeOnTarget(target, cb, inParams, current);
}



IceGrid::RegistryReplicaAdminRouter::RegistryReplicaAdminRouter(const string& name,
                                                                const DatabasePtr& database) :
    _name(name),
    _database(database)
{
}

void
IceGrid::RegistryReplicaAdminRouter::ice_invoke_async(const AMD_Object_ice_invokePtr& cb,
                                                      const std::pair<const Ice::Byte*, const Ice::Byte*>& inParams,
                                                      const Current& current)
{
    ObjectPrx target;

    if(current.id.name == _name)
    {
        // Straight to the local Admin object
        target = current.adapter->getCommunicator()->getAdmin();
    }
    else
    {
        try
        {
            // Forward to Admin object in remote replica
            target = _database->getReplica(current.id.name)->getAdminProxy();
        }
        catch(const RegistryNotExistException&)
        {
        }
    }

    if(target == 0)
    {
        throw ObjectNotExistException(__FILE__, __LINE__);
    }

    target = target->ice_facet(current.facet);

    invokeOnTarget(target, cb, inParams, current);
}
