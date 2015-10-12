<!--Title: Profiles and Child Containers-->
<!--Url: profiles-and-child-containers-->

<div class="alert alert-info" role="alert"><i>Child Containers</i> are <b>not</b> interchangeable with <i>Nested Containers</i>. See <linkto:the-container/nested-containers]> for more information on nested containers.</div>


The single best source for information about the particulars of child container behavior is to look through the acceptance tests for [child containers](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Acceptance/child_containers.cs) and
[profiles](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Configuration/DSL/profiles_acceptance_tester.cs).
from the code in GitHub.

## Child Containers

The easiest way to explain a child container is to just show it in action:

<[sample:show_a_child_container_in_action]>

_Child Container's_ are a mechanism to make a completely new `Container` that can override some of the parent `Container's` registrations but still
fall back to the parent `Container` to fulfill any request that is not explicitly configured to the child container. The behavior of a child container
in how it resolves services and allows you to override the parent container is very similar to a <[linkto:the-container/nested-containers;title=nested container]>, but the crucial difference is in how the two concepts handle lifecycles and <[linkto:the-container/disposing;title=calling IDisposable.Dispose()]>.

A couple salient facts about child containers that **should** (knock on wood) dispel the confusion about when and why to use them versus a nested container:

1. Child Containers do **not** change how the <[linkto:object-lifecycle/supported-lifecycles;title=default   transient lifecycle behaves]>.
1. Child Containers (and Profiles) were intended to be used to establish different service resolutions for different subsystems of the running system or 
   to isolate registration overrides for specific types of users or customers (multi-tenancy).



## Profiles

**Profiles were completely redesigned as part of the big 3.0 release**.

_Profiles_ are just named child containers that may be configured upfront through `Registry` configurations. 
Profiles are one of the oldest features that date back to the very beginning of StructureMap. Originally profile's were conceived of as
a way to vary StructureMap registrations by development environment as the code moved from running locally on a developer's box to testing
servers to production. While that usage is still valid, it is probably more common to use profiles to define overrides for how StructureMap
should resolve services in different modes of the application (connected vs offline) or different types of system users.

<[sample:profile-in-action]>

## Child Containers and Singletons

If you register a new `Instance` directly to a child container, that registration is really scoped as a
singleton within the usage of that particular child container:

<[sample:singletons_to_child_container_are_isolated]>


## Creating a Nested Container from a Child Container

It is perfectly valid to create a nested container from a child container for short-lived requests or transactions:

<[sample:nested_container_from_child]>


## Example: Child Containers in Automated Testing

My shop has been somewhat successful in writing automated integration tests in a [whitebox testing style](https://en.wikipedia.org/wiki/White-box_testing)
where we happily "swap out" a handful of external services with a [stubbed service](https://en.wikipedia.org/wiki/Method_stub) that we can happily control
to either setup system state or measure the interaction of our system with the stub (outgoing emails etc) -- with the canonical
example being a web service that our biggest application has to call for authentication purposes that is frequently unavailable during
our development efforts and not quite reliable even when it is available:\.

That's great, and the stubs certainly help the testing efforts be more productive. Until the stateful stubs we inject in one test end up bleeding into
another test as de facto shared state and making those tests fail in ways that were very difficult to diagnose. To combat this problem of test isolation,
we've introduced the idea of using a clean child container per integration test so that the test harness tools can happily
swap in stubs without impacting other tests in the test suite.

In action, that usage looks something like this:

<[sample:stubs-with-child-containers]>

